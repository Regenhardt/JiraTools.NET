namespace JiraLib.Models;

/// <summary>
/// Base entity for all jira entities.
/// </summary>
/// <param name="Id">Internal ID of this entity.</param>
/// <param name="Self">API-link to this entity.</param>
public record JiraBaseEntity(int Id, Uri Self);