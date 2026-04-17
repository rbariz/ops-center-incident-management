using OpsCenterIncidentManagement.Contracts.Incidents;

namespace OpsCenterIncidentManagement.Contracts.Dashboard
{
    public sealed record OpsDashboardResponse(
    Guid DemoIncidentId,
    int TotalIncidents,
    int EscalatedIncidents,
    IReadOnlyList<IncidentResponse> LatestIncidents
);
}
