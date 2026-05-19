namespace profitcore_backend.Domain.Health
{
    public sealed record HealthCheckResult(string Status, DateTime CheckedAtUtc);
}
