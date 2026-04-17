using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Domain.Entities;
using OpsCenterIncidentManagement.Domain.Enums;
using OpsCenterIncidentManagement.Domain.Exceptions;

namespace OpsCenterIncidentManagement.Application.UseCases.StartIncidentWork
{
   
    public sealed class StartIncidentWorkHandler
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IIncidentTimelineEventRepository _timelineRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public StartIncidentWorkHandler(
            IIncidentRepository incidentRepository,
            IIncidentTimelineEventRepository timelineRepository,
            IUnitOfWork unitOfWork,
            IClock clock)
        {
            _incidentRepository = incidentRepository;
            _timelineRepository = timelineRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task HandleAsync(Guid incidentId, CancellationToken cancellationToken = default)
        {
            var incident = await _incidentRepository.GetByIdAsync(incidentId, cancellationToken);
            if (incident is null)
                throw new DomainException("Incident not found.");

            var nowUtc = _clock.UtcNow;

            incident.StartWork();

            var timelineEvent = new IncidentTimelineEvent(
                Guid.NewGuid(),
                incident.Id,
                TimelineEventType.StatusChanged,
                "Incident moved to InProgress.",
                nowUtc);

            await _timelineRepository.AddAsync(timelineEvent, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
