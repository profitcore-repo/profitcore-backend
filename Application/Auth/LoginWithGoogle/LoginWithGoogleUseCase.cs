using profitcore_backend.Domain.Auth.Google;
using profitcore_backend.Domain.Auth.Google.Ports;

namespace profitcore_backend.Application.Auth.LoginWithGoogle
{
    public interface ILoginWithGoogleUseCase
    {
        Task<GoogleUser> ExecuteAsync(string idToken, CancellationToken cancellationToken);
    }

    public sealed class LoginWithGoogleUseCase : ILoginWithGoogleUseCase
    {
        private readonly IGoogleIdTokenValidator _validator;

        public LoginWithGoogleUseCase(IGoogleIdTokenValidator validator)
        {
            _validator = validator;
        }

        public Task<GoogleUser> ExecuteAsync(string idToken, CancellationToken cancellationToken)
        {
            return _validator.ValidateAsync(idToken, cancellationToken);
        }
    }
}
