namespace JiraLib.Jira;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public class JiraFields
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public JiraStatus Status { get; set; }

    [JsonPropertyName("issuetype")]
    public JiraIssueType IssueType { get; set; }

    [JsonPropertyName("issuelinks")] public List<IssueLink>? Links { get; set; }

    public JsonNode Worklog { get; set; }
}
