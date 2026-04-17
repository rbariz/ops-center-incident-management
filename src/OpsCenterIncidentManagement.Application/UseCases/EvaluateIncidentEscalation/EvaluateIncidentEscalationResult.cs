namespace OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;

public sealed record EvaluateIncidentEscalationResult(
    Guid IncidentId,
    bool IsEscalated,
    string Reason
);
