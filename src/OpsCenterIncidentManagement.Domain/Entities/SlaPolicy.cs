using OpsCenterIncidentManagement.Domain.Enums;
using OpsCenterIncidentManagement.Domain.Exceptions;

namespace OpsCenterIncidentManagement.Domain.Entities;

public sealed class SlaPolicy
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public IncidentPriority Priority { get; private set; }
    public int EscalationAfterMinutes { get; private set; }

    private SlaPolicy() { }

    public SlaPolicy(Guid id, string name, IncidentPriority priority, int escalationAfterMinutes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("SLA policy name is required.");

        if (escalationAfterMinutes <= 0)
            throw new DomainException("Escalation delay must be greater than zero.");

        Id = id;
        Name = name;
        Priority = priority;
        EscalationAfterMinutes = escalationAfterMinutes;
    }
}
