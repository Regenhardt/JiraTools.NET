// ReSharper disable UnusedMember.Global

namespace JiraLib.Models;

/// <summary>
/// A jira issue with all fields loaded.
/// </summary>
/// <param name="Id">Internal ID for this issue, e.g. 123456.</param>
/// <param name="Self">API-Link to this issue.</param>
/// <param name="Key">Public key for this issue, e.g. "ABC-123".</param>
/// <param name="Fields">Fields of this issue.</param>
public record JiraIssue(int Id,  Uri Self, string Key, IssueFields Fields): MinimalIssue(Id, Self, Key, Fields)
{
    /// <summary>
    /// Whether or not this issue is closed.
    /// </summary>
    public bool IsClosed() => Fields.Status.Name == "closed";

    /// <inheritdoc/>
    public override string ToString() => $"{Key}: {Fields.Summary}";
}