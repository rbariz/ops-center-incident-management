using OpsCenterIncidentManagement.Domain.Entities;
using OpsCenterIncidentManagement.Domain.Enums;

namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface ISlaPolicyRepository
{
    Task<SlaPolicy?> GetByPriorityAsync(IncidentPriority priority, CancellationToken cancellationToken = default);
}
