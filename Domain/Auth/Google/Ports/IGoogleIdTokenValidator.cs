namespace profitcore_backend.Domain.Auth.Google.Ports
{
    public interface IGoogleIdTokenValidator
    {
        Task<Google.GoogleUser> ValidateAsync(string idToken, CancellationToken cancellationToken);
    }
}
