namespace JiraLib.Models;

/// <summary>
/// Represents a jira user. For deserialization.
/// </summary>
/// <param name="Self">API-Link to this user.</param>
/// <param name="Name">Name of this user.</param>
/// <param name="Key">Unique internal key of this user.</param>
/// <param name="DisplayName">Name for displaying this user.</param>
/// <param name="Active">Whether or not this user is active.</param>
/// <param name="TimeZone">Which timezone this user lives or works in, e.g. "Europe/Berlin".</param>
/// <param name="AvatarUrls">URIs to user avatars using the size as keys, e.g. "16x16" or "48x48". These include the scheme and may include query parameters. May or may not end in a file extension.</param>
public record JiraUser(Uri Self, string Name, string Key, string DisplayName, bool Active, string TimeZone, Dictionary<string, Uri> AvatarUrls);