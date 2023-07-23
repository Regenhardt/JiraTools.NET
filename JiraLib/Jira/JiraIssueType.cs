namespace JiraLib.Jira;

using System.Text.Json.Serialization;

/// <summary>
/// Type of a jira issue and the type's metadata.
/// </summary>
public class JiraIssueType
{
    /// <summary>
    /// Name of this issue type, e.g. Story
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Whether or not this issue type is a type of subtask
    /// </summary>
    [JsonPropertyName("subtask")]
    public bool IsSubtask { get; set; }

    /// <summary>
    /// Whether or not this issue type is a type of epic
    /// </summary>
    /// <returns></returns>
    public bool IsEpic()
    {
        return Name.ToLower() == "epic";
    }
}
