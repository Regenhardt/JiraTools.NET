namespace JiraLib;

using System.Text.Json.Serialization;

public class IssueLink
{
    [JsonPropertyName("type")]
    public JiraLinkType Type { get; set; } = null!;

    public string LinkType => Direction == LinkDirection.Outward ? Type.Outward : Type.Inward;
    
    public string Target => OutwardIssue?.Key ?? InwardIssue?.Key ?? string.Empty;

    [JsonPropertyName("outwardIssue")]
    public JiraIssue? OutwardIssue { get; set; }

    [JsonPropertyName("inwardIssue")]
    public JiraIssue? InwardIssue { get; set; }

    public LinkDirection Direction => OutwardIssue != null ? LinkDirection.Outward : LinkDirection.Inward;
}

public enum LinkDirection
{
    Inward,
    Outward
}
