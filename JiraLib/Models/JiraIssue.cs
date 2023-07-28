// ReSharper disable UnusedMember.Global

namespace JiraLib.Models;

/// <summary>
/// A jira issue.
/// </summary>
/// <param name="Key">Public key for this ticket, e.g. "ABC-123".</param>
/// <param name="Id">Internal ID for this ticket, e.g. "123456".</param>
/// <param name="Self">API-Link to this ticket.</param>
/// <param name="Fields">Fields of this ticket.</param>
public record JiraIssue(string Key, int Id, Uri Self, IssueFields Fields)
{
    /// <summary>
    /// Whether or not this issue is closed.
    /// </summary>
    public bool IsClosed() => Fields.Status.Name == "closed";

    /// <inheritdoc/>
    public override string ToString() => $"{Key}: {Fields.Summary}";
}
