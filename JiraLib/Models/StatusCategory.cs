namespace JiraLib.Models;

/// <summary>
/// Category of a jira ticket's status. These can be "ToDo", "InProgress" or "Done"
/// </summary>
/// <param name="Self">API-link to this status category.</param>
/// <param name="Id">Internal ID of this category.</param>
/// <param name="Key">Unique key of this category, e.g. "done".</param>
/// <param name="ColorName">Name of the color of this category, i.e. "grey", "blue" or "green".</param>
/// <param name="Name">Name of this category, e.g. "Done" or "Fertig".</param>
public record StatusCategory(Uri Self, int Id, string Key, string ColorName, string Name);