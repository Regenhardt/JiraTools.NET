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
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = null!;

    /// <summary>
    /// Status of the jira ticket, e.g. "In Progress".
    /// </summary>
    [JsonPropertyName("status")]
    public JiraStatus Status { get; set; } = null!;

    [JsonPropertyName("issuetype")] public JiraIssueType IssueType { get; set; } = null!;

    [JsonPropertyName("issuelinks")] public List<IssueLink>? Links { get; set; } = null!;

    public JsonNode Worklog { get; set; } = null!;
}
