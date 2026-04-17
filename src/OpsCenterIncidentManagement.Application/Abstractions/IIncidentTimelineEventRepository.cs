using OpsCenterIncidentManagement.Domain.Entities;

namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface IIncidentTimelineEventRepository
{
    Task AddAsync(IncidentTimelineEvent timelineEvent, CancellationToken cancellationToken = default);
}
