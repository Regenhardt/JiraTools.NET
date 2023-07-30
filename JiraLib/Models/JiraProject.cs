namespace JiraLib.Models;

/// <summary>
/// A jira project.
/// </summary>
/// <param name="Self">API-Link to this project.</param>
/// <param name="Id">Internal ID of this project.</param>
/// <param name="Key">Public key of this project, e.g. "ABC".</param>
/// <param name="Name">Name of this project, e.g. "My Project".</param>
/// <param name="ProjectTypeKey">Type of this project, e.g. "software".</param>
/// <param name="AvatarUrls">URIs to project avatars using the size as keys, e.g. "16x16" or "48x48". These include the scheme and may include query parameters. May or may not end in a file extension.</param>
/// <param name="ProjectCategory">Category of this project.</param>
public record JiraProject(Uri Self, int Id, string Key, string Name, string ProjectTypeKey, Dictionary<string, Uri> AvatarUrls, ProjectCategory ProjectCategory): JiraBaseEntity(Id, Self);