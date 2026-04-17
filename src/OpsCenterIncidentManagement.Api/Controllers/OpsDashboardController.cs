using Microsoft.AspNetCore.Mvc;
using OpsCenterIncidentManagement.Application.UseCases.GetOpsDashboard;
using OpsCenterIncidentManagement.Contracts.Dashboard;
using OpsCenterIncidentManagement.Infrastructure.DependencyInjection;

namespace OpsCenterIncidentManagement.Api.Controllers
{
    [ApiController]
    [Route("api/ops-dashboard")]
    public sealed class OpsDashboardController : ControllerBase
    {
        private readonly GetOpsDashboardHandler _handler;

        public OpsDashboardController(GetOpsDashboardHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        [ProducesResponseType(typeof(OpsDashboardResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _handler.HandleAsync(InMemorySeed.DemoIncidentId, cancellationToken);
            return Ok(result);
        }
    }
}
