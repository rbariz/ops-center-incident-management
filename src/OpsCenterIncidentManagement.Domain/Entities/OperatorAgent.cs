using OpsCenterIncidentManagement.Domain.Enums;
using OpsCenterIncidentManagement.Domain.Exceptions;

namespace OpsCenterIncidentManagement.Domain.Entities;

public sealed class OperatorAgent
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Team { get; private set; }
    public AgentStatus Status { get; private set; }

    private OperatorAgent() { }

    public OperatorAgent(Guid id, string name, string team)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Agent name is required.");

        if (string.IsNullOrWhiteSpace(team))
            throw new DomainException("Team is required.");

        Id = id;
        Name = name;
        Team = team;
        Status = AgentStatus.Available;
    }

    public void MarkBusy()
    {
        Status = AgentStatus.Busy;
    }

    public void MarkAvailable()
    {
        Status = AgentStatus.Available;
    }

    public void MarkOffline()
    {
        Status = AgentStatus.Offline;
    }
}
