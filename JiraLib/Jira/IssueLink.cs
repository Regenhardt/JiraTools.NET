namespace JiraLib.Jira;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a link between two issues.
/// </summary>
public class IssueLink
{
    /// <summary>
    /// The type of link.
    /// </summary>
    [JsonPropertyName("type")]
    public JiraLinkType Type { get; set; } = null!;

    /// <summary>
    /// The name of the link in its current context, e.g. "is blocked by" or "blocks" depending on the direction.
    /// </summary>
    public string DirectedLinkName => Direction == LinkDirection.Outward ? Type.Outward : Type.Inward;

    /// <summary>
    /// The issue this link connects to.
    /// </summary>
    public string Target => OutwardIssue?.Key ?? InwardIssue?.Key ?? string.Empty;

    /// <summary>
    /// The issue this link connects to if it is an outward link, otherwise null.
    /// </summary>
    [JsonPropertyName("outwardIssue")]
    public JiraIssue? OutwardIssue { get; set; }

    /// <summary>
    /// The issue this link connects to if it is an inward link, otherwise null.
    /// </summary>
    [JsonPropertyName("inwardIssue")]
    public JiraIssue? InwardIssue { get; set; }

    /// <summary>
    /// The direction in which the link is active (e.g. is called "blocks" vs. "is blocked by").
    /// </summary>
    public LinkDirection Direction => OutwardIssue != null ? LinkDirection.Outward : LinkDirection.Inward;
}

/// <summary>
/// Directions in which a link can be directed.
/// </summary>
public enum LinkDirection
{
    /// <summary>
    /// Link is directed inwards i.e. the current issue is the target of the link.
    /// </summary>
    /// <example>Example link name: "blocks"</example>
    Inward,

    /// <summary>
    /// Link is directed outwards i.e. the current issue is the source of the link.
    /// </summary>
    /// <example>Example link name: "is blocked by"</example>
    Outward
}
