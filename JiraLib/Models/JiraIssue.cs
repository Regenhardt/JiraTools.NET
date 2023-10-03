// ReSharper disable UnusedMember.Global

namespace JiraLib.Models;

using System.Text.RegularExpressions;

/// <summary>
/// A jira issue with all fields loaded.
/// </summary>
/// <param name="Id">Internal ID for this issue, e.g. 123456.</param>
/// <param name="Self">API-Link to this issue.</param>
/// <param name="Key">Public key for this issue, e.g. "ABC-123".</param>
/// <param name="Fields">Fields of this issue.</param>
/// <param name="Names">If expanded, contains names of the fields with the key being the fields ID (e.g. "customfield_12345").</param>
public record JiraIssue(int Id,  Uri Self, string Key, IssueFields Fields, Dictionary<string, string>? Names = null)
{
    private static readonly Regex EpicNamesRegex = new("^[Ee]pic[ -]?(?:[Ll]ink|[Vv]erknüpfung)?$");

    /// <summary>
    /// Whether or not this issue is closed.
    /// </summary>
    public bool IsClosed() => Fields.Status.IsClosed;

    /// <summary>
    /// The issue ID of the epic this issue belongs to, if any.
    /// </summary>
    public string? Epic => Names?.Keys.FirstOrDefault(k => EpicNamesRegex.IsMatch(Names[k])) is { } epicKey
        ? Fields.AdditionalFields.TryGetValue(epicKey, out var epicId) ? epicId.ToString() : null
        : null;

    /// <inheritdoc/>
    public override string ToString() => $"{Key}: {Fields.Summary}";

    /// <summary>
    /// Implicitly convert a <see cref="JiraIssue"/> to a <see cref="MinimalIssue"/>.
    /// </summary>
    public static implicit operator MinimalIssue(JiraIssue issue) => new(issue.Id, issue.Self, issue.Key, issue.Fields);
}