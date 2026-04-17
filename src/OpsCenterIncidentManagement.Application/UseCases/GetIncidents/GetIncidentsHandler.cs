using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Contracts.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpsCenterIncidentManagement.Application.UseCases.GetIncidents
{
    public sealed class GetIncidentsHandler
    {
        private readonly IIncidentRepository _incidentRepository;

        public GetIncidentsHandler(IIncidentRepository incidentRepository)
        {
            _incidentRepository = incidentRepository;
        }

        public async Task<IReadOnlyList<IncidentResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var items = await _incidentRepository.GetAllAsync(cancellationToken);

            return items
                .OrderByDescending(x => x.CreatedAtUtc)
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
        }
    }
}
