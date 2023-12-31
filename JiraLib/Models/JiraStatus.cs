﻿namespace JiraLib.Models;

/// <summary>
/// Status of a jira ticket.
/// </summary>
/// <param name="Name">Name of the status, e.g. "closed".</param>
/// <param name="Self">API-link to this status.</param>
/// <param name="Description">Full sentence description of this status.</param>
/// <param name="Iconurl">URl to an icon as a quick visual cue about this status.</param>
/// <param name="Id">Internal ID of this status.</param>
/// <param name="StatusCategory">Which category this status is in, e.g. done or in progress.</param>
public record JiraStatus(string Name, Uri Self, string Description, Uri Iconurl, int Id, StatusCategory StatusCategory): JiraBaseEntity(Id, Self)
{
    private static readonly string[] ClosedStatuses = { "closed", "done", "resolved", "fertig", "it fertig", "für update bereit", "für update freigegeben", "qm erfolgt", "geschlossen" };

    /// <summary>
    /// Whether or not this status is a closed status.
    /// </summary>
    public bool IsClosed => ClosedStatuses.Contains(Name.ToLower());
}