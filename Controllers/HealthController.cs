using Microsoft.AspNetCore.Mvc;
using profitcore_backend.Application.Health;
using profitcore_backend.Domain.Health;

namespace profitcore_backend.Controllers
{
    [ApiController]
    [Route("health")]
    public sealed class HealthController : ControllerBase
    {
        private readonly IGetHealthCheckUseCase _useCase;

        public HealthController(IGetHealthCheckUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpGet]
        public async Task<ActionResult<HealthCheckResult>> Get(CancellationToken cancellationToken)
        {
            var result = await _useCase.ExecuteAsync(cancellationToken);
            return Ok(result);
        }
    }
}
