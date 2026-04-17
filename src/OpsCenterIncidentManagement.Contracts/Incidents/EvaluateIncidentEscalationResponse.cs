namespace OpsCenterIncidentManagement.Contracts.Incidents
{
    public sealed record EvaluateIncidentEscalationResponse(
    Guid IncidentId,
    bool IsEscalated,
    string Reason
);

}
