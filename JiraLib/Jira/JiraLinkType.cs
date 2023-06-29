namespace JiraLib.Jira;

using System.Text.Json.Serialization;

/// <summary>
/// Describes a link type on an issue
/// </summary>
public class JiraLinkType
{
    /// <summary>
    /// General name of the link type, e.g. Cloners
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Inward/Indirect name of link type, e.g. is cloned by
    /// </summary>
    [JsonPropertyName("inward")]
    public string Inward { get; set; } = null!;

    /// <summary>
    /// Outward/Direct name of link type, e.g. clones
    /// </summary>
    [JsonPropertyName("outward")]
    public string Outward { get; set; } = null!;
}
