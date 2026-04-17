namespace OpsCenterIncidentManagement.Contracts.Incidents
{
    public sealed record IncidentTimelineEventResponse(
    Guid Id,
    Guid IncidentId,
    string EventType,
    string Description,
    DateTime OccurredAtUtc
);

}
