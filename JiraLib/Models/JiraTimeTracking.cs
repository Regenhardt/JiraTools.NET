namespace JiraLib.Models;

/// <summary>
/// The time tracking information for a Jira issue.
/// </summary>
/// <param name="OriginalEstimate">The original estimate for the issue, e.g. "2h 30m".</param>
/// <param name="RemainingEstimate">The remaining estimate for the issue, e.g. "1h 30m".</param>
/// <param name="TimeSpent">The time spent on the issue, e.g. "1h".</param>
/// <param name="OriginalEstimateSeconds">The original estimate for the issue in seconds.</param>
/// <param name="RemainingEstimateSeconds">The remaining estimate for the issue in seconds.</param>
/// <param name="TimeSpentSeconds">The time spent on the issue in seconds.</param>
public record JiraTimeTracking(string OriginalEstimate, string RemainingEstimate, string TimeSpent, uint OriginalEstimateSeconds, uint RemainingEstimateSeconds, uint TimeSpentSeconds);