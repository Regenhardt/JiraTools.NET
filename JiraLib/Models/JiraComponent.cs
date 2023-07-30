namespace JiraLib.Models;

/// <summary>
/// Represents a jira issue component. For deserialization.
/// </summary>
/// <param name="Name">Name of the component, displayed on the UI.</param>
/// <param name="Id">Internal ID of the component.</param>
/// <param name="Self">API-Link to this component.</param>
public record JiraComponent(string Name, int Id, Uri Self) : JiraBaseEntity(Id, Self);