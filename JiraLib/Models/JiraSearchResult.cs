namespace JiraLib.Models;

using System.Collections.Generic;

/// <summary>
/// Represents the result of a jira search. For deserialization.
/// </summary>
/// <param name="Issues">List of issues found by the search.</param>
public record JiraSearchResult(List<JiraIssue> Issues);
