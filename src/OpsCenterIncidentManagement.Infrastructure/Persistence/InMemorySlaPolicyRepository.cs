using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Domain.Entities;
using OpsCenterIncidentManagement.Domain.Enums;

namespace OpsCenterIncidentManagement.Infrastructure.Persistence
{
    public sealed class InMemorySlaPolicyRepository : ISlaPolicyRepository
    {
        private readonly InMemoryStore _store;

        public InMemorySlaPolicyRepository(InMemoryStore store)
        {
            _store = store;
        }

        public Task<SlaPolicy?> GetByPriorityAsync(IncidentPriority priority, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_store.SlaPolicies.FirstOrDefault(x => x.Priority == priority));
        }
    }
}
