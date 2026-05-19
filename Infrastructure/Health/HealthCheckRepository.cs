using profitcore_backend.Domain.Health;
using profitcore_backend.Domain.Health.Ports;

namespace profitcore_backend.Infrastructure.Health
{
    public sealed class HealthCheckRepository : IHealthCheckRepository
    {
        public Task<HealthCheckResult> GetAsync(CancellationToken cancellationToken)
        {
            var result = new HealthCheckResult("ok", DateTime.UtcNow);
            return Task.FromResult(result);
        }
    }
}
