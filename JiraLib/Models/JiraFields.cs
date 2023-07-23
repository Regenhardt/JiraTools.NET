// ReSharper disable UnusedMember.Global

namespace JiraLib.Models;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

/// <summary>
/// Represents all the fields of a jira issue. For deserialization.
/// </summary>
/// <param name="Summary">Summary of the jira issue as found in its title.</param>
/// <param name="Status">Status of the jira issue, e.g. "In Progress".</param>
/// <param name="IssueType">Type of the jira issue, e.g. "Story".</param>
/// <param name="Priority">Priority of the jira issue, e.g. "High".</param>
/// <param name="Creator">User that created this issue. This is automatically set when the issue is created and cannot be changed.</param>
/// <param name="Reporter">User that reported this issue. Set to the creator by default but can be changed in order to create an issue for another user.</param>
/// <param name="Assignee">User this issue is assigned to.</param>
/// <param name="JiraProject">Project this issue belongs to.</param>
/// <param name="Description">Description of this issue.</param>
/// <param name="Created">When this issue was created.</param>
/// <param name="Updated">When this issue was last updated.</param>
/// <param name="ResolutionDate">When this issue was resolved, if at all.</param>
/// <param name="FixVersions">Which versions this issue was fixed in, if any.</param>
/// <param name="Links">Links this issue has to other issues. Does not include subtasks unless explicitly linked.</param>
public record JiraFields(
    string Summary,
    JiraStatus Status,
    JiraIssueType IssueType,
    IssuePriority Priority,
    JiraUser Creator,
    JiraUser Reporter,
    JiraUser Assignee,
    JiraProject JiraProject,
    string Description,
    DateTime Created,
    DateTime Updated,
    DateTime? ResolutionDate,
    List<FixVersion>? FixVersions,
    [property: JsonPropertyName("issuelinks")] List<IssueLink>? Links)
{
    public JsonNode Worklog { get; set; } = null!;
}