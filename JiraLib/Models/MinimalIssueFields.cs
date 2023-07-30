namespace JiraLib.Models;

/// <summary>
/// Minimal set of fields always loaded for an issue.
/// </summary>
/// <param name="Summary">Summary of the jira issue as found in its title.</param>
/// <param name="Status">Status of the jira issue, e.g. "In Progress".</param>
/// <param name="Priority">Priority of the jira issue, e.g. "High".</param>
/// <param name="IssueType">Type of the jira issue, e.g. "Story".</param>
public record MinimalIssueFields(string Summary, JiraStatus Status, IssuePriority Priority, JiraIssueType IssueType);