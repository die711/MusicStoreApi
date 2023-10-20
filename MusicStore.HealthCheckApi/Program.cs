using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MusicStore.DataAccess;
using MusicStore.HealthCheckApi.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MusicStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MusicStoreDb"));
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "api" })
    .AddDbContextCheck<MusicStoreDbContext>("Database", HealthStatus.Unhealthy, tags: new[] { "database", "api" })
    .AddTypeActivatedCheck<PingHealthCheck>("Firebase", HealthStatus.Degraded, tags: new[] { "api" }, "firebase.com")
    .AddTypeActivatedCheck<PingHealthCheck>("Azure", HealthStatus.Degraded, tags: new[] { "api" }, "azure.com")
    .AddTypeActivatedCheck<PingHealthCheck>("Pokemon API", HealthStatus.Degraded, tags: new[] { "api" }, "pokeapi.co");

var app = builder.Build();

app.UseHttpsRedirection();


app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    Predicate = x => x.Tags.Contains("api")
});

app.Run();