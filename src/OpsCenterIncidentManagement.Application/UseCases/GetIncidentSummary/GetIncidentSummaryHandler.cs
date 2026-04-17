using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Contracts.Incidents;
using OpsCenterIncidentManagement.Domain.Enums;

namespace OpsCenterIncidentManagement.Application.UseCases.GetIncidentSummary
{
    public sealed class GetIncidentSummaryHandler
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ISlaPolicyRepository _slaPolicyRepository;
        private readonly IIncidentTimelineEventRepository _timelineRepository;
        private readonly IClock _clock;

        public GetIncidentSummaryHandler(
            IIncidentRepository incidentRepository,
            ISlaPolicyRepository slaPolicyRepository,
            IIncidentTimelineEventRepository timelineRepository,
            IClock clock)
        {
            _incidentRepository = incidentRepository;
            _slaPolicyRepository = slaPolicyRepository;
            _timelineRepository = timelineRepository;
            _clock = clock;
        }

        public async Task<IncidentSummaryResponse?> HandleAsync(
            Guid incidentId,
            CancellationToken cancellationToken = default)
        {
            var incident = await _incidentRepository.GetByIdAsync(incidentId, cancellationToken);
            if (incident is null)
                return null;

            var policy = await _slaPolicyRepository.GetByPriorityAsync(incident.Priority, cancellationToken);
            var timeline = await _timelineRepository.GetByIncidentIdAsync(incident.Id, cancellationToken);

            var nowUtc = _clock.UtcNow;
            var ageMinutes = (int)Math.Max(0, (nowUtc - incident.CreatedAtUtc).TotalMinutes);

            int? slaMinutes = policy?.EscalationAfterMinutes;
            DateTime? escalationDueAtUtc = slaMinutes.HasValue
                ? incident.CreatedAtUtc.AddMinutes(slaMinutes.Value)
                : null;

            var isFinalized = incident.Status is IncidentStatus.Resolved or IncidentStatus.Closed or IncidentStatus.Cancelled;

            var isOverdue = escalationDueAtUtc.HasValue &&
                            nowUtc >= escalationDueAtUtc.Value &&
                            incident.Status != IncidentStatus.Escalated &&
                            !isFinalized;

            return new IncidentSummaryResponse(
                incident.Id,
                incident.Title,
                incident.Description,
                incident.Location,
                incident.Priority.ToString().ToLowerInvariant(),
                incident.Status.ToString().ToLowerInvariant(),
                incident.CreatedAtUtc,
                incident.AssignedAtUtc,
                incident.ResolvedAtUtc,
                ageMinutes,
                slaMinutes,
                escalationDueAtUtc,
                isOverdue,
                timeline.Count
            );
        }
    }
}
