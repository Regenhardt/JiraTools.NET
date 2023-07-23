// ReSharper disable UnusedMember.Global
namespace JiraLib.Jira;

/// <summary>
/// A jira issue.
/// </summary>
public class JiraIssue
{
    /// <summary>
    /// Public key for this ticket, e.g. "ABC-123".
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Internal ID for this ticket, e.g. "123456".
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// API URI to this issue.
    /// </summary>
    public Uri Self { get; set; } = null!;

    /// <summary>
    /// Contains all the fields of this issue.
    /// </summary>
    public JiraFields Fields { get; set; } = new();

    /// <summary>
    /// Whether or not this issue is closed.
    /// </summary>
    public bool IsClosed() => Fields.Status.Name == "closed";

    /// <inheritdoc/>
    public override string ToString() => $"{Key}: {Fields.Summary}";
}
