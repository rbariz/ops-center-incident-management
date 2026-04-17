using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidents;
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
        private readonly EvaluateIncidentEscalationHandler _evaluateIncidentEscalationHandler;

        public IncidentsController(
            GetIncidentsHandler getIncidentsHandler,
            GetIncidentTimelineHandler getIncidentTimelineHandler,
            EvaluateIncidentEscalationHandler evaluateIncidentEscalationHandler)
        {
            _getIncidentsHandler = getIncidentsHandler;
            _getIncidentTimelineHandler = getIncidentTimelineHandler;
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
