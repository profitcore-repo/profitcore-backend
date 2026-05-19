namespace profitcore_backend.Domain.Health.Ports
{
    public interface IHealthCheckRepository
    {
        Task<HealthCheckResult> GetAsync(CancellationToken cancellationToken);
    }
}
