using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Domain.Entities;
using OpsCenterIncidentManagement.Domain.Enums;
using OpsCenterIncidentManagement.Domain.Exceptions;

namespace OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;

public sealed class EvaluateIncidentEscalationHandler
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ISlaPolicyRepository _slaPolicyRepository;
    private readonly IIncidentTimelineEventRepository _timelineRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public EvaluateIncidentEscalationHandler(
        IIncidentRepository incidentRepository,
        ISlaPolicyRepository slaPolicyRepository,
        IIncidentTimelineEventRepository timelineRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _incidentRepository = incidentRepository;
        _slaPolicyRepository = slaPolicyRepository;
        _timelineRepository = timelineRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<EvaluateIncidentEscalationResult> HandleAsync(
        EvaluateIncidentEscalationRequest request,
        CancellationToken cancellationToken = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId, cancellationToken);
        if (incident is null)
            throw new DomainException("Incident not found.");

        if (incident.Status is IncidentStatus.Resolved or IncidentStatus.Closed or IncidentStatus.Cancelled)
        {
            return new EvaluateIncidentEscalationResult(
                incident.Id,
                false,
                "Incident is already finalized.");
        }

        if (incident.Status == IncidentStatus.Escalated)
        {
            return new EvaluateIncidentEscalationResult(
                incident.Id,
                false,
                "Incident is already escalated.");
        }

        var policy = await _slaPolicyRepository.GetByPriorityAsync(incident.Priority, cancellationToken);
        if (policy is null)
            throw new DomainException("SLA policy not found for incident priority.");

        var nowUtc = _clock.UtcNow;
        var escalationDueAt = incident.CreatedAtUtc.AddMinutes(policy.EscalationAfterMinutes);

        if (nowUtc < escalationDueAt)
        {
            return new EvaluateIncidentEscalationResult(
                incident.Id,
                false,
                "SLA threshold not reached yet.");
        }

        incident.Escalate();

        var timelineEvent = new IncidentTimelineEvent(
            Guid.NewGuid(),
            incident.Id,
            TimelineEventType.Escalated,
            $"Incident escalated automatically after SLA threshold ({policy.EscalationAfterMinutes} min).",
            nowUtc);

        await _timelineRepository.AddAsync(timelineEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EvaluateIncidentEscalationResult(
            incident.Id,
            true,
            "Incident escalated.");
    }
}
