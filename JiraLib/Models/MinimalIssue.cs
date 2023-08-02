namespace JiraLib.Models;

using System.Text.Json.Serialization;

/// <summary>
/// A jira issue with only the fields that are always loaded.
/// </summary>
/// <param name="Id">Internal ID for this issue, e.g. 123456.</param>
/// <param name="Self">API-Link to this issue.</param>
/// <param name="Key">Public key for this issue, e.g. "ABC-123".</param>
/// <param name="Fields">Fields of this issue.</param>
public record MinimalIssue(int Id, Uri Self, string Key, MinimalIssueFields Fields)
{
    /// <inheritdoc/>
    public override string ToString() => $"{Key}: {Fields.Summary}";
}