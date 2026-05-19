namespace profitcore_backend.Domain.Auth.Google
{
    public sealed record GoogleUser(
        string Subject,
        string? Email,
        bool EmailVerified,
        string? Name,
        string? PictureUrl,
        DateTime ExpiresAtUtc
    );
}
