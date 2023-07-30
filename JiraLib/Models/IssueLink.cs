namespace JiraLib.Models;

/// <summary>
/// Represents a link between two issues.
/// </summary>
/// <param name="Id">Internal ID of the link.</param>
/// <param name="Self">API-Link to this link.</param>
/// <param name="Type">The type of link.</param>
/// <param name="OutwardIssue">The issue this link connects to if it is an outward link, otherwise null. As part of an <seealso cref="IssueLink"/> object, linked issues are loaded in minimal format. Load the issue directly to get the full field set.</param>
/// <param name="InwardIssue">The issue this link connects to if it is an inward link, otherwise null. As part of an <seealso cref="IssueLink"/> object, linked issues are loaded in minimal format. Load the issue directly to get the full field set.</param>
public record IssueLink(int Id, Uri Self, JiraLinkType Type, MinimalIssue? OutwardIssue, MinimalIssue? InwardIssue) : JiraBaseEntity(Id, Self)
{
    /// <summary>
    /// The name of the link in its current context, e.g. "is blocked by" or "blocks" depending on the direction.
    /// </summary>
    public string DirectedLinkName => Direction == LinkDirection.Outward ? Type.Outward : Type.Inward;

    /// <summary>
    /// The issue this link connects to.
    /// </summary>
    public string Target => OutwardIssue?.Key ?? InwardIssue?.Key ?? string.Empty;
    
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
