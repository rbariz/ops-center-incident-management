param(
    [string]$Root = "D:\SAASs\ops-center-incident-management\src\OpsCenterIncidentManagement.Domain"
)

function Write-CodeFile {
    param(
        [string]$Path,
        [string]$Content
    )

    $directory = Split-Path $Path -Parent
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
    Set-Content -Path $Path -Value $Content -Encoding UTF8
}

$EntitiesPath = Join-Path $Root "Entities"
$EnumsPath = Join-Path $Root "Enums"
$ExceptionsPath = Join-Path $Root "Exceptions"

# =========================
# Exceptions/DomainException.cs
# =========================
Write-CodeFile -Path (Join-Path $ExceptionsPath "DomainException.cs") -Content @'
namespace OpsCenterIncidentManagement.Domain.Exceptions;

public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
'@

# =========================
# Enums/IncidentStatus.cs
# =========================
Write-CodeFile -Path (Join-Path $EnumsPath "IncidentStatus.cs") -Content @'
namespace OpsCenterIncidentManagement.Domain.Enums;

public enum IncidentStatus
{
    Created = 1,
    Assigned = 2,
    InProgress = 3,
    Escalated = 4,
    Resolved = 5,
    Closed = 6,
    Cancelled = 7
}
'@

# =========================
# Enums/IncidentPriority.cs
# =========================
Write-CodeFile -Path (Join-Path $EnumsPath "IncidentPriority.cs") -Content @'
namespace OpsCenterIncidentManagement.Domain.Enums;

public enum IncidentPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}
'@

# =========================
# Enums/TimelineEventType.cs
# =========================
Write-CodeFile -Path (Join-Path $EnumsPath "TimelineEventType.cs") -Content @'
namespace OpsCenterIncidentManagement.Domain.Enums;

public enum TimelineEventType
{
    Created = 1,
    Assigned = 2,
    StatusChanged = 3,
    Escalated = 4,
    NoteAdded = 5,
    Resolved = 6,
    Closed = 7
}
'@

# =========================
# Enums/AgentStatus.cs
# =========================
Write-CodeFile -Path (Join-Path $EnumsPath "AgentStatus.cs") -Content @'
namespace OpsCenterIncidentManagement.Domain.Enums;

public enum AgentStatus
{
    Available = 1,
    Busy = 2,
    Offline = 3
}
'@

# =========================
# Entities/OperatorAgent.cs
# =========================
Write-CodeFile -Path (Join-Path $EntitiesPath "OperatorAgent.cs") -Content @'
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
'@

# =========================
# Entities/SlaPolicy.cs
# =========================
Write-CodeFile -Path (Join-Path $EntitiesPath "SlaPolicy.cs") -Content @'
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
'@

# =========================
# Entities/Incident.cs
# =========================
Write-CodeFile -Path (Join-Path $EntitiesPath "Incident.cs") -Content @'
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
'@

# =========================
# Entities/IncidentAssignment.cs
# =========================
Write-CodeFile -Path (Join-Path $EntitiesPath "IncidentAssignment.cs") -Content @'
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
'@

# =========================
# Entities/IncidentTimelineEvent.cs
# =========================
Write-CodeFile -Path (Join-Path $EntitiesPath "IncidentTimelineEvent.cs") -Content @'
using OpsCenterIncidentManagement.Domain.Enums;
using OpsCenterIncidentManagement.Domain.Exceptions;

namespace OpsCenterIncidentManagement.Domain.Entities;

public sealed class IncidentTimelineEvent
{
    public Guid Id { get; private set; }
    public Guid IncidentId { get; private set; }
    public TimelineEventType EventType { get; private set; }
    public string Description { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }

    private IncidentTimelineEvent() { }

    public IncidentTimelineEvent(
        Guid id,
        Guid incidentId,
        TimelineEventType eventType,
        string description,
        DateTime occurredAtUtc)
    {
        if (incidentId == Guid.Empty)
            throw new DomainException("IncidentId is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description is required.");

        Id = id;
        IncidentId = incidentId;
        EventType = eventType;
        Description = description;
        OccurredAtUtc = occurredAtUtc;
    }
}
'@

Write-Host "OpsCenter Domain scaffold generated successfully." -ForegroundColor Green