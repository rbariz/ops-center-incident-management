using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidents;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidentSummary;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidentTimeline;
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

        public IncidentsController(
            GetIncidentsHandler getIncidentsHandler,
            GetIncidentTimelineHandler getIncidentTimelineHandler,
            GetIncidentSummaryHandler getIncidentSummaryHandler,
            EvaluateIncidentEscalationHandler evaluateIncidentEscalationHandler)
        {
            _getIncidentsHandler = getIncidentsHandler;
            _getIncidentTimelineHandler = getIncidentTimelineHandler;
            _getIncidentSummaryHandler = getIncidentSummaryHandler;
            _evaluateIncidentEscalationHandler = evaluateIncidentEscalationHandler;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<IncidentResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _getIncidentsHandler.HandleAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{incidentId:guid}/timeline")]
        [ProducesResponseType(typeof(IReadOnlyList<IncidentTimelineEventResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTimeline(Guid incidentId, CancellationToken cancellationToken)
        {
            var result = await _getIncidentTimelineHandler.HandleAsync(incidentId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{incidentId:guid}/summary")]
        [ProducesResponseType(typeof(IncidentSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSummary(Guid incidentId, CancellationToken cancellationToken)
        {
            var result = await _getIncidentSummaryHandler.HandleAsync(incidentId, cancellationToken);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("{incidentId:guid}/evaluate-escalation")]
        [ProducesResponseType(typeof(EvaluateIncidentEscalationResponse), StatusCodes.Status200OK)]
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
