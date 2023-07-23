namespace JiraLib.Models;

/// <summary>
/// A version an issue was fixed in.
/// </summary>
/// <param name="Self">API-Link to this version</param>
/// <param name="Id">Internal ID of this version</param>
/// <param name="Description">Description of this version (might be empty because who describes their versions?)</param>
/// <param name="Name">Name of the version, e.g. "0.5"</param>
/// <param name="Archived">Whether or not this version is archived</param>
/// <param name="Released">Whether or not this version is released</param>
public record FixVersion(string Self, string Id, string Description, string Name, bool Archived, bool Released);