namespace JiraLib;

using System.Text.Json.Serialization;

public class JiraLinkType
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("inward")]
    public string Inward { get; set; } = null!;

    [JsonPropertyName("outward")]
    public string Outward { get; set; } = null!;
}
