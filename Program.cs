using profitcore_backend.Application.Auth.LoginWithGoogle;
using profitcore_backend.Domain.Auth.Google.Ports;
using profitcore_backend.Infrastructure.Auth.Google;
using profitcore_backend.Application.Health;
using profitcore_backend.Domain.Health.Ports;
using profitcore_backend.Infrastructure.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection(GoogleAuthOptions.SectionName));
builder.Services.AddSingleton<IGoogleIdTokenValidator, GoogleIdTokenValidator>();
builder.Services.AddScoped<ILoginWithGoogleUseCase, LoginWithGoogleUseCase>();

builder.Services.AddScoped<IHealthCheckRepository, HealthCheckRepository>();
builder.Services.AddScoped<IGetHealthCheckUseCase, GetHealthCheckUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
