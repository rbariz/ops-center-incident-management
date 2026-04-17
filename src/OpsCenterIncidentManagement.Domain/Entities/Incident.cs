using OpsCenterIncidentManagement.Domain.Enums;
using OpsCenterIncidentManagement.Domain.Exceptions;

namespace OpsCenterIncidentManagement.Domain.Entities;

public sealed class Incident
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Location { get; private set; }
    public IncidentPriority Priority { get; private set; }
    public IncidentStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? AssignedAtUtc { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }

    private Incident() { }

    public Incident(
        Guid id,
        string title,
        string description,
        string location,
        IncidentPriority priority,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description is required.");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Location is required.");

        Id = id;
        Title = title;
        Description = description;
        Location = location;
        Priority = priority;
        CreatedAtUtc = createdAtUtc;
        Status = IncidentStatus.Created;
    }

    public void Assign(DateTime nowUtc)
    {
        if (Status != IncidentStatus.Created && Status != IncidentStatus.Escalated)
            throw new DomainException("Incident cannot be assigned in its current state.");

        Status = IncidentStatus.Assigned;
        AssignedAtUtc = nowUtc;
    }

    public void StartWork()
    {
        if (Status != IncidentStatus.Assigned)
            throw new DomainException("Only assigned incidents can start.");

        Status = IncidentStatus.InProgress;
    }

    public void Escalate()
    {
        if (Status == IncidentStatus.Resolved || Status == IncidentStatus.Closed || Status == IncidentStatus.Cancelled)
            throw new DomainException("Closed incidents cannot be escalated.");

        Status = IncidentStatus.Escalated;
    }

    public void Resolve(DateTime nowUtc)
    {
        if (Status != IncidentStatus.Assigned &&
            Status != IncidentStatus.InProgress &&
            Status != IncidentStatus.Escalated)
            throw new DomainException("Incident cannot be resolved in its current state.");

        Status = IncidentStatus.Resolved;
        ResolvedAtUtc = nowUtc;
    }

    public void Close(DateTime nowUtc)
    {
        if (Status != IncidentStatus.Resolved)
            throw new DomainException("Only resolved incidents can be closed.");

        Status = IncidentStatus.Closed;
        ClosedAtUtc = nowUtc;
    }

    public void Cancel()
    {
        if (Status == IncidentStatus.Closed)
            throw new DomainException("Closed incidents cannot be cancelled.");

        Status = IncidentStatus.Cancelled;
    }
}
