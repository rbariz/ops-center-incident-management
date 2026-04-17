using OpsCenterIncidentManagement.Domain.Exceptions;

namespace OpsCenterIncidentManagement.Domain.Entities;

public sealed class IncidentAssignment
{
    public Guid Id { get; private set; }
    public Guid IncidentId { get; private set; }
    public Guid AgentId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }

    private IncidentAssignment() { }

    public IncidentAssignment(Guid id, Guid incidentId, Guid agentId, DateTime assignedAtUtc)
    {
        if (incidentId == Guid.Empty)
            throw new DomainException("IncidentId is required.");

        if (agentId == Guid.Empty)
            throw new DomainException("AgentId is required.");

        Id = id;
        IncidentId = incidentId;
        AgentId = agentId;
        AssignedAtUtc = assignedAtUtc;
    }
}
