namespace JiraLib.Models;

/// <summary>
/// Base class for all jira tasks.
/// </summary>
/// <param name="Id">Internal ID of the task.</param>
/// <param name="Self">API-Link to this task.</param>
/// <param name="Key">Public key for this task, e.g. "ABC-123".</param>
/// <param name="BaseFields">Fields of this task.</param>
public record JiraBaseTask(int Id, Uri Self, string Key, BaseTaskFields BaseFields) : JiraBaseEntity(Id, Self);