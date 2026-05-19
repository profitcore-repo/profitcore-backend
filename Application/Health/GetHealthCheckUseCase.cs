using profitcore_backend.Domain.Health;
using profitcore_backend.Domain.Health.Ports;

namespace profitcore_backend.Application.Health
{
    public interface IGetHealthCheckUseCase
    {
        Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken);
    }

    public sealed class GetHealthCheckUseCase : IGetHealthCheckUseCase
    {
        private readonly IHealthCheckRepository _repository;

        public GetHealthCheckUseCase(IHealthCheckRepository repository)
        {
            _repository = repository;
        }

        public Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _repository.GetAsync(cancellationToken);
        }
    }
}
