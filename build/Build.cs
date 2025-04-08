using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[ShutdownDotNetAfterServerBuild]
[DotNetVerbosityMapping]
[GitHubActions(
    "continuous",
    GitHubActionsImage.WindowsLatest,
    AutoGenerate = false,
    OnPullRequestBranches = new[] { MasterBranch, ReleaseBranch },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(Verify), nameof(Cover), nameof(Pack) },
    EnableGitHubToken = true)]
[GitHubActions(
    "release",
    GitHubActionsImage.WindowsLatest,
    AutoGenerate = false,
    OnPushTags = new[] { "v*" },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(Verify), nameof(Cover), nameof(Publish) },
    EnableGitHubToken = true,
    ImportSecrets = new[] { Secrets.NuGetApiKey })]
partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    const string MasterBranch = "master";
    const string ReleaseBranch = "release/*";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    [CI] readonly GitHubActions GitHubActions;

    [Parameter("GitHub auth token", Name = "github-token"), Secret] readonly string GitHubToken;
    [Parameter("Forces the continuous integration build flag")] readonly bool CI;

    [Secret][Parameter("NuGet API Key (secret)", Name = Secrets.NuGetApiKey)] readonly string NuGetApiKey;
    readonly string NuGetSource = "https://api.nuget.org/v3/index.json";

    IEnumerable<Project> Excluded => new[]
    {
        Solution.GetProject("_build"),
        Solution.GetProject("TestTypeFoundation")
    };

    IEnumerable<Project> TestProjects => Solution.GetAllProjects("*Tests");
    IEnumerable<Project> Libraries => Solution.Projects.Except(TestProjects).Except(Excluded);
    IEnumerable<Project> CSharpLibraries => Libraries.Where(x => x.Is(ProjectType.CSharpProject));
    IEnumerable<AbsolutePath> Packages => PackagesDirectory.GlobFiles("*.nupkg");

    bool IsContinuousIntegration => IsServerBuild || CI;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "testresults";
    AbsolutePath ReportsDirectory => ArtifactsDirectory / "reports";
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory
                .GlobDirectories("**/bin", "**/obj")
                .ForEach(x => x.DeleteDirectory());

            ArtifactsDirectory.DeleteDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Verify => _ => _
        .DependsOn(Restore)
        .Before(Compile)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Verify)
                .SetNoRestore(FinishedTargets.Contains(Restore))
                .SetContinuousIntegrationBuild(IsContinuousIntegration));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetDeterministic(IsContinuousIntegration)
                .SetContinuousIntegrationBuild(IsContinuousIntegration)
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetNoRestore(FinishedTargets.Contains(Restore)));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultsDirectory / "*.zip")
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetResultsDirectory(TestResultsDirectory)
                .SetNoBuild(FinishedTargets.Contains(Compile))
                .When(_ => InvokedTargets.Contains(Cover), _ => _
                    .AddProcessAdditionalArguments("--")
                    .AddProcessAdditionalArguments("--coverage")
                    .AddProcessAdditionalArguments("--coverage-output", TestResultsDirectory / "coverage.cobertura.xml")
                    .AddProcessAdditionalArguments("--coverage-output-format", "cobertura")));

            var testArchive = TestResultsDirectory / "TestResults.zip";
            testArchive.DeleteFile();
            TestResultsDirectory.CompressTo(testArchive);
        });

    Target Cover => _ => _
        .DependsOn(Test)
        .Consumes(Test)
        .Before(Pack)
        .Produces(ReportsDirectory / "*.zip")
        .Executes(() =>
        {
            ReportGenerator(_ => _
                .SetFramework("net5.0")
                .SetAssemblyFilters("-TestTypeFoundation*")
                .SetReports(TestResultsDirectory / "**" / "coverage.cobertura.xml")
                .SetTargetDirectory(ReportsDirectory)
                .SetReportTypes("lcov", ReportTypes.HtmlInline));

            var coverageArchive = ReportsDirectory / "CoverageReport.zip";
            coverageArchive.DeleteFile();
            ReportsDirectory.CompressTo(coverageArchive);
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Consumes(Compile)
        .After(Test)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetConfiguration(Configuration)
                .SetNoBuild(FinishedTargets.Contains(Compile))
                .SetOutputDirectory(PackagesDirectory)
                .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
                .EnableIncludeSymbols()
                .SetDeterministic(IsContinuousIntegration)
                .SetContinuousIntegrationBuild(IsContinuousIntegration)
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .CombineWith(CSharpLibraries, (s, p) => s.SetProject(p)));
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .Executes(() =>
        {
            DotNetNuGetPush(s => s
                .EnableSkipDuplicate()
                .When(
                    _ => GitHubActions.IsOnSemVerTag(),
                    v => v
                        .SetApiKey(NuGetApiKey)
                        .SetSource(NuGetSource))
                .CombineWith(Packages, (_, p) => _.SetTargetPath(p)));
        });

    public static class Secrets
    {
        public const string NuGetApiKey = "NUGET_API_KEY";
    }
}