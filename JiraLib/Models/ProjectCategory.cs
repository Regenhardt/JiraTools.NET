namespace JiraLib.Models;

/// <summary>
/// A category for a jira project.
/// </summary>
/// <param name="Self">API-Link to this category.</param>
/// <param name="Id">Internal ID of this category.</param>
/// <param name="Name">Name of this category, e.g. "Active development project".</param>
/// <param name="Description">Full sentence description of this category.</param>
public record ProjectCategory(Uri Self, int Id, string Name, string Description);