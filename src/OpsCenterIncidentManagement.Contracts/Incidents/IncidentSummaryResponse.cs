namespace OpsCenterIncidentManagement.Contracts.Incidents
{
    public sealed record IncidentSummaryResponse(
    Guid Id,
    string Title,
    string Description,
    string Location,
    string Priority,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? AssignedAtUtc,
    DateTime? ResolvedAtUtc,
    int AgeMinutes,
    int? SlaEscalationAfterMinutes,
    DateTime? EscalationDueAtUtc,
    bool IsOverdueForEscalation,
    int TimelineEventsCount
);

}
