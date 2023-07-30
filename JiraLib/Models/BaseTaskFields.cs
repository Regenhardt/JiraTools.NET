namespace JiraLib.Models;

/// <summary>
/// Collection of fields that are common to both <see cref="JiraIssue"/> and <see cref="JiraSubtask"/>.
/// </summary>
/// <param name="Summary">Summary of the jira issue as found in its title.</param>
/// <param name="Status">Status of the jira issue, e.g. "In Progress".</param>
/// <param name="IssueType">Type of the jira issue, e.g. "Story".</param>
/// <param name="Priority">Priority of the jira issue, e.g. "High".</param>
public record BaseTaskFields(string Summary, JiraStatus Status, JiraIssueType IssueType, IssuePriority Priority);