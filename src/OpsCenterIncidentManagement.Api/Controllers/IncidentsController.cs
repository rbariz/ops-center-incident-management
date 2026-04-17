using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpsCenterIncidentManagement.Application.UseCases.AssignIncident;
using OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidents;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidentSummary;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidentTimeline;
using OpsCenterIncidentManagement.Application.UseCases.ResolveIncident;
using OpsCenterIncidentManagement.Application.UseCases.StartIncidentWork;
using OpsCenterIncidentManagement.Contracts.Incidents;

namespace OpsCenterIncidentManagement.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public sealed class IncidentsController : ControllerBase
    {
        private readonly GetIncidentsHandler _getIncidentsHandler;
        private readonly GetIncidentTimelineHandler _getIncidentTimelineHandler;
        private readonly GetIncidentSummaryHandler _getIncidentSummaryHandler;
        private readonly EvaluateIncidentEscalationHandler _evaluateIncidentEscalationHandler;
        private readonly AssignIncidentHandler _assignIncidentHandler;
        private readonly StartIncidentWorkHandler _startIncidentWorkHandler;
        private readonly ResolveIncidentHandler _resolveIncidentHandler;

        public IncidentsController(
            GetIncidentsHandler getIncidentsHandler,
            GetIncidentTimelineHandler getIncidentTimelineHandler,
            GetIncidentSummaryHandler getIncidentSummaryHandler,
            EvaluateIncidentEscalationHandler evaluateIncidentEscalationHandler,
            AssignIncidentHandler assignIncidentHandler,
            StartIncidentWorkHandler startIncidentWorkHandler,
            ResolveIncidentHandler resolveIncidentHandler)
        {
            _getIncidentsHandler = getIncidentsHandler;
            _getIncidentTimelineHandler = getIncidentTimelineHandler;
            _getIncidentSummaryHandler = getIncidentSummaryHandler;
            _evaluateIncidentEscalationHandler = evaluateIncidentEscalationHandler;
            _assignIncidentHandler = assignIncidentHandler;
            _startIncidentWorkHandler = startIncidentWorkHandler;
            _resolveIncidentHandler = resolveIncidentHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _getIncidentsHandler.HandleAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{incidentId:guid}/timeline")]
        public async Task<IActionResult> GetTimeline(Guid incidentId, CancellationToken cancellationToken)
        {
            var result = await _getIncidentTimelineHandler.HandleAsync(incidentId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{incidentId:guid}/summary")]
        public async Task<IActionResult> GetSummary(Guid incidentId, CancellationToken cancellationToken)
        {
            var result = await _getIncidentSummaryHandler.HandleAsync(incidentId, cancellationToken);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("{incidentId:guid}/assign")]
        public async Task<IActionResult> Assign(Guid incidentId, CancellationToken cancellationToken)
        {
            await _assignIncidentHandler.HandleAsync(incidentId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{incidentId:guid}/start")]
        public async Task<IActionResult> Start(Guid incidentId, CancellationToken cancellationToken)
        {
            await _startIncidentWorkHandler.HandleAsync(incidentId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{incidentId:guid}/resolve")]
        public async Task<IActionResult> Resolve(Guid incidentId, CancellationToken cancellationToken)
        {
            await _resolveIncidentHandler.HandleAsync(incidentId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{incidentId:guid}/evaluate-escalation")]
        public async Task<IActionResult> EvaluateEscalation(Guid incidentId, CancellationToken cancellationToken)
        {
            var result = await _evaluateIncidentEscalationHandler.HandleAsync(
                new EvaluateIncidentEscalationRequest(incidentId),
                cancellationToken);

            var response = new EvaluateIncidentEscalationResponse(
                result.IncidentId,
                result.IsEscalated,
                result.Reason);

            return Ok(response);
        }
    }
}
