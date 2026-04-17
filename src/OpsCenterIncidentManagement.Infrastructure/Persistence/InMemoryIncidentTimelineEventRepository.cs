using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Domain.Entities;

namespace OpsCenterIncidentManagement.Infrastructure.Persistence
{
    public sealed class InMemoryIncidentTimelineEventRepository : IIncidentTimelineEventRepository
    {
        private readonly InMemoryStore _store;

        public InMemoryIncidentTimelineEventRepository(InMemoryStore store)
        {
            _store = store;
        }

        public Task AddAsync(IncidentTimelineEvent timelineEvent, CancellationToken cancellationToken = default)
        {
            _store.TimelineEvents.Add(timelineEvent);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<IncidentTimelineEvent>> GetByIncidentIdAsync(Guid incidentId, CancellationToken cancellationToken = default)
        {
            var items = _store.TimelineEvents
                .Where(x => x.IncidentId == incidentId)
                .OrderByDescending(x => x.OccurredAtUtc)
                .ToList();

            return Task.FromResult<IReadOnlyList<IncidentTimelineEvent>>(items);
        }
    }
}
