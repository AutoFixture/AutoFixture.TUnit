using System.Text.RegularExpressions;

namespace Nuke.Common.CI.GitHubActions;

public static class GitHubActionsExtensions
{
    private static readonly Regex SemVerRef = new(@"^refs\/tags\/v(?<version>\d+\.\d+\.\d+)", RegexOptions.Compiled);

    public static bool IsOnSemVerTag(this GitHubActions source)
    {
        return !string.IsNullOrWhiteSpace(source?.Ref)
            && SemVerRef.IsMatch(source.Ref);
    }
}