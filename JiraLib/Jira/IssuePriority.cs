namespace JiraLib.Jira;

/// <summary>
/// Priority of a jira issue.
/// </summary>
/// <param name="Self">API-Link to this priority</param>
/// <param name="IconUrl">API-Link to the icon of this priority, normally a direct link to an svg image.</param>
/// <param name="Name">Name of this priority, e.g. "High"</param>
/// <param name="Id">Internal ID of this priority.</param>
public record IssuePriority(Uri Self, Uri IconUrl, string Name, int Id);