using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using profitcore_backend.Domain.Auth.Google;
using profitcore_backend.Domain.Auth.Google.Ports;

namespace profitcore_backend.Infrastructure.Auth.Google
{
    public sealed class GoogleIdTokenValidator : IGoogleIdTokenValidator
    {
        private static readonly string[] ValidIssuers =
        [
            "https://accounts.google.com",
            "accounts.google.com"
        ];

        private readonly GoogleAuthOptions _options;
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        public GoogleIdTokenValidator(IOptions<GoogleAuthOptions> options)
        {
            _options = options.Value;

            var metadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
            var documentRetriever = new HttpDocumentRetriever { RequireHttps = true };
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                metadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
        }

        public async Task<GoogleUser> ValidateAsync(string idToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(idToken))
            {
                throw new ArgumentException("idToken is required", nameof(idToken));
            }

            if (_options.Audiences.Length == 0)
            {
                throw new InvalidOperationException("Authentication:Google:Audiences must be configured.");
            }

            var configuration = await _configurationManager.GetConfigurationAsync(cancellationToken);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = ValidIssuers,
                ValidateAudience = true,
                ValidAudiences = _options.Audiences,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = configuration.SigningKeys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                RequireSignedTokens = true
            };

            var principal = _tokenHandler.ValidateToken(idToken, parameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwt)
            {
                throw new SecurityTokenException("Invalid token type.");
            }

            var subject = principal.FindFirstValue("sub");
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new SecurityTokenException("Token missing sub claim.");
            }

            var email = principal.FindFirstValue("email");
            var name = principal.FindFirstValue("name");
            var picture = principal.FindFirstValue("picture");
            var emailVerified = ParseBool(principal.FindFirstValue("email_verified"));

            return new GoogleUser(
                Subject: subject,
                Email: email,
                EmailVerified: emailVerified,
                Name: name,
                PictureUrl: picture,
                ExpiresAtUtc: jwt.ValidTo.ToUniversalTime()
            );
        }

        private static bool ParseBool(string? value)
        {
            return value is not null && bool.TryParse(value, out var parsed) && parsed;
        }
    }
}
