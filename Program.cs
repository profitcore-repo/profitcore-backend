using profitcore_backend.Application.Auth.LoginWithGoogle;
using profitcore_backend.Domain.Auth.Google.Ports;
using profitcore_backend.Infrastructure.Auth.Google;
using profitcore_backend.Application.Health;
using profitcore_backend.Domain.Health.Ports;
using profitcore_backend.Infrastructure.Health;
using profitcore_backend.Infrastructure.Errors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);

        if (!context.ProblemDetails.Extensions.ContainsKey("code"))
        {
            var code = context.ProblemDetails.Status switch
            {
                StatusCodes.Status400BadRequest => "validation_error",
                StatusCodes.Status401Unauthorized => "unauthorized",
                StatusCodes.Status403Forbidden => "forbidden",
                StatusCodes.Status404NotFound => "not_found",
                _ when context.ProblemDetails.Status >= StatusCodes.Status500InternalServerError => "unexpected_error",
                _ => "bad_request"
            };

            context.ProblemDetails.Extensions["code"] = code;
        }
    };
});
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection(GoogleAuthOptions.SectionName));
builder.Services.AddSingleton<IGoogleIdTokenValidator, GoogleIdTokenValidator>();
builder.Services.AddScoped<ILoginWithGoogleUseCase, LoginWithGoogleUseCase>();

builder.Services.AddScoped<IHealthCheckRepository, HealthCheckRepository>();
builder.Services.AddScoped<IGetHealthCheckUseCase, GetHealthCheckUseCase>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
