namespace JiraLib.Jira;

using System.Text.Json.Serialization;

public class JiraIssue
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = null!;

    /// <summary>
    /// API URI to this issue.
    /// </summary>
    [JsonPropertyName("self")]
    public Uri Self { get; set; } = null!;

    [JsonPropertyName("fields")] public JiraFields Fields { get; set; } = new();

    public bool IsClosed() => Fields.Status.Name == "closed";
}
