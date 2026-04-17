using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Domain.Entities;

namespace OpsCenterIncidentManagement.Infrastructure.Persistence
{
    public sealed class InMemoryIncidentRepository : IIncidentRepository
    {
        private readonly InMemoryStore _store;

        public InMemoryIncidentRepository(InMemoryStore store)
        {
            _store = store;
        }

        public Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_store.Incidents.FirstOrDefault(x => x.Id == id));
        }

        public Task<IReadOnlyList<Incident>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = _store.Incidents
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToList();

            return Task.FromResult<IReadOnlyList<Incident>>(items);
        }
    }
}
