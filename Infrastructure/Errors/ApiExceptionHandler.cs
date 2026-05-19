using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace profitcore_backend.Infrastructure.Errors
{
    public sealed class ApiExceptionHandler : IExceptionHandler
    {
        private static readonly ActivitySource ActivitySource = new("profitcore_backend");
        private readonly IWebHostEnvironment _environment;

        public ApiExceptionHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, title, code) = Map(exception);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode >= StatusCodes.Status500InternalServerError && !_environment.IsDevelopment()
                    ? "Ocorreu um erro inesperado."
                    : exception.Message
            };

            problemDetails.Extensions["code"] = code;
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            var activity = Activity.Current ?? ActivitySource.StartActivity("UnhandledException");
            if (activity is not null)
            {
                problemDetails.Extensions["spanId"] = activity.SpanId.ToString();
            }

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        private static (int StatusCode, string Title, string Code) Map(Exception exception)
        {
            return exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida", "bad_request"),
                SecurityTokenException => (StatusCodes.Status401Unauthorized, "Token inválido", "invalid_token"),
                InvalidOperationException => (StatusCodes.Status500InternalServerError, "Falha de configuração", "configuration_error"),
                HttpRequestException => (StatusCodes.Status503ServiceUnavailable, "Falha ao chamar serviço externo", "external_service_error"),
                _ => (StatusCodes.Status500InternalServerError, "Erro inesperado", "unexpected_error")
            };
        }
    }
}
