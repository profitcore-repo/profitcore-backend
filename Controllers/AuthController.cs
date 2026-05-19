using Microsoft.AspNetCore.Mvc;
using profitcore_backend.Application.Auth.LoginWithGoogle;
using profitcore_backend.Domain.Auth.Google;

namespace profitcore_backend.Controllers
{
    [ApiController]
    [Route("auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly ILoginWithGoogleUseCase _loginWithGoogle;

        public AuthController(ILoginWithGoogleUseCase loginWithGoogle)
        {
            _loginWithGoogle = loginWithGoogle;
        }

        public sealed record GoogleLoginRequest(string IdToken);

        [HttpPost("google")]
        public async Task<ActionResult<GoogleUser>> Google([FromBody] GoogleLoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _loginWithGoogle.ExecuteAsync(request.IdToken, cancellationToken);
            return Ok(user);
        }
    }
}
