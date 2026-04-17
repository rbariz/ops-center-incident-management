namespace OpsCenterIncidentManagement.Contracts.Incidents
{
    public sealed record IncidentResponse(
    Guid Id,
    string Title,
    string Location,
    string Priority,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? AssignedAtUtc,
    DateTime? ResolvedAtUtc,
    DateTime? ClosedAtUtc
);

}
