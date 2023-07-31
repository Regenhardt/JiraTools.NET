// ReSharper disable UnusedMember.Global

namespace JiraLib.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents all the fields of a fully loaded jira issue. For deserialization.
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
/// <param name="Components">List of components of this jira issue, if any.</param>
/// <param name="Subtasks">List of subtasks of this jira issue, if any. As part of a <seealso cref="JiraIssue"/>, subtasks are loaded in minimal format only. Load the task directly to get the full field set.</param>
/// <param name="Links">Links this issue has to other issues. Does not include subtasks unless explicitly linked.</param>
public record IssueFields(
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
    List<JiraComponent>? Components,
    List<MinimalIssue>? Subtasks,
    [property: JsonPropertyName("issuelinks")]
    List<IssueLink>? Links): MinimalIssueFields(Summary, Status, Priority, IssueType)
{
    /// <summary>
    /// To deserialize subtasks from the newer rest API, we need to use a different property name (even though we don't actually use the newer rest API yet).
    /// </summary>
    [JsonPropertyName("sub-tasks")]
    public List<MinimalIssue>? SubtasksAlt
    {
        init
        {
            if(value !=null) 
                Subtasks = value;
        }
    }
}