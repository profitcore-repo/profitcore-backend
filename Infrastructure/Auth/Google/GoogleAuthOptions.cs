namespace profitcore_backend.Infrastructure.Auth.Google
{
    public sealed class GoogleAuthOptions
    {
        public const string SectionName = "Authentication:Google";

        public string[] Audiences { get; init; } = [];
    }
}
