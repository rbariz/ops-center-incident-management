using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Contracts.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpsCenterIncidentManagement.Application.UseCases.GetIncidentTimeline
{
    public sealed class GetIncidentTimelineHandler
    {
        private readonly IIncidentTimelineEventRepository _timelineRepository;

        public GetIncidentTimelineHandler(IIncidentTimelineEventRepository timelineRepository)
        {
            _timelineRepository = timelineRepository;
        }

        public async Task<IReadOnlyList<IncidentTimelineEventResponse>> HandleAsync(
            Guid incidentId,
            CancellationToken cancellationToken = default)
        {
            var items = await _timelineRepository.GetByIncidentIdAsync(incidentId, cancellationToken);

            return items
                .OrderByDescending(x => x.OccurredAtUtc)
                .Select(x => new IncidentTimelineEventResponse(
                    x.Id,
                    x.IncidentId,
                    x.EventType.ToString().ToLowerInvariant(),
                    x.Description,
                    x.OccurredAtUtc))
                .ToList();
        }
    }
}
