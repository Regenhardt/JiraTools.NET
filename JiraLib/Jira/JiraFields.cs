// ReSharper disable UnusedMember.Global
namespace JiraLib.Jira;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

/// <summary>
/// Represents all the fields of a jira ticket. For deserialization.
/// </summary>
public class JiraFields
{
    /// <summary>
    /// Summary of the jira ticket as found in its title.
    /// </summary>
    public string Summary { get; set; } = null!;

    /// <summary>
    /// Status of the jira ticket, e.g. "In Progress".
    /// </summary>
    public JiraStatus Status { get; set; } = null!;

    /// <summary>
    /// What type of issue this is.
    /// </summary>
    public JiraIssueType IssueType { get; set; } = null!;

    /// <summary>
    /// Links this issue has to other issues. Does not include subtasks unless explicitly linked.
    /// </summary>
    [JsonPropertyName("issuelinks")]
    public List<IssueLink>? Links { get; set; } = null!;

    public JsonNode Worklog { get; set; } = null!;

    /// <summary>
    /// Which versions this issue was fixed in.
    /// </summary>
    public List<FixVersion> FixVersions { get; set; } = null!;

    /// <summary>
    /// Priority of this issue.
    /// </summary>
    public IssuePriority Priority { get; set; } = null!;
}