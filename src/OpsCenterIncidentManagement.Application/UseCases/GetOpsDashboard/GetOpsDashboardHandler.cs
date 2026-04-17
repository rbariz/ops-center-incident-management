using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Contracts.Dashboard;
using OpsCenterIncidentManagement.Contracts.Incidents;
using OpsCenterIncidentManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpsCenterIncidentManagement.Application.UseCases.GetOpsDashboard
{
    
    public sealed class GetOpsDashboardHandler
    {
        private readonly IIncidentRepository _incidentRepository;

        public GetOpsDashboardHandler(IIncidentRepository incidentRepository)
        {
            _incidentRepository = incidentRepository;
        }

        public async Task<OpsDashboardResponse> HandleAsync(
            Guid demoIncidentId,
            CancellationToken cancellationToken = default)
        {
            var items = await _incidentRepository.GetAllAsync(cancellationToken);

            var latest = items
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(20)
                .Select(x => new IncidentResponse(
                    x.Id,
                    x.Title,
                    x.Location,
                    x.Priority.ToString().ToLowerInvariant(),
                    x.Status.ToString().ToLowerInvariant(),
                    x.CreatedAtUtc,
                    x.AssignedAtUtc,
                    x.ResolvedAtUtc,
                    x.ClosedAtUtc))
                .ToList();

            return new OpsDashboardResponse(
                demoIncidentId,
                items.Count,
                items.Count(x => x.Status == IncidentStatus.Escalated),
                latest);
        }
    }
}
