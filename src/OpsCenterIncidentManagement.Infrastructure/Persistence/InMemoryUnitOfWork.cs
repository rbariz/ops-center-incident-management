using OpsCenterIncidentManagement.Application.Abstractions;

namespace OpsCenterIncidentManagement.Infrastructure.Persistence
{
    public sealed class InMemoryUnitOfWork : IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
