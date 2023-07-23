namespace JiraLib.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Type of a jira issue and the type's metadata.
/// </summary>
/// <param name="Name">Name of this issue type, e.g. Story</param>
/// <param name="IsSubtask">Whether or not this issue type is a type of subtask</param>
/// <param name="Self">API-Link to this issue type.</param>
/// <param name="Description">Full sentence description of this issue type.</param>
/// <param name="IconUrl">URI to the icon of this issue type, including scheme and possibly query parameters. May or may not end in a file extension.</param>
/// <param name="AvatarId">Internal ID of the icon of this issue type.</param>
/// <param name="Id">Internal ID of this issue type.</param>
public record JiraIssueType(string Name, [property: JsonPropertyName("subtask")] bool IsSubtask, Uri Self, string Description, Uri IconUrl, int AvatarId, int Id)
{
    /// <summary>
    /// Whether or not this issue type is a type of epic
    /// </summary>
    /// <returns></returns>
    public bool IsEpic()
    {
        return Name.ToLower() == "epic";
    }
}