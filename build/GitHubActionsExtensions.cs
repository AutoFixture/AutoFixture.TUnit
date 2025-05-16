using System.Text.RegularExpressions;
using Nuke.Common.CI.GitHubActions;

public static partial class GitHubActionsExtensions
{
    private static readonly Regex SemVerRef = GetSemVerRegex();

    public static bool IsOnSemVerTag(this GitHubActions source)
    {
        return !string.IsNullOrWhiteSpace(source?.Ref)
            && SemVerRef.IsMatch(source.Ref);
    }

    [GeneratedRegex(@"^refs\/tags\/v(?<version>\d+\.\d+\.\d+)", RegexOptions.Compiled)]
    private static partial Regex GetSemVerRegex();
}