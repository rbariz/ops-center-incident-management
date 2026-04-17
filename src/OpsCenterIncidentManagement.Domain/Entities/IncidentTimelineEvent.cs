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
