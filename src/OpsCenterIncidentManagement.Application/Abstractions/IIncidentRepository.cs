using OpsCenterIncidentManagement.Domain.Entities;

namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Incident>> GetAllAsync(CancellationToken cancellationToken = default);
}
