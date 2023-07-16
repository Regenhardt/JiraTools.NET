namespace JiraLib.Jira;

using System.Text.Json.Serialization;

#nullable disable

/// <summary>
/// Status of a jira ticket. For deserialization.
/// </summary>
public class JiraStatus
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
