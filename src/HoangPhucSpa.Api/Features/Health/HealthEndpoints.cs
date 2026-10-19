using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HoangPhucSpa.Api.Features.Health;

public sealed record HealthResponse(string Status);

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("/health");

        // Liveness: không đụng DB
        group.MapGet("/live", () => TypedResults.Ok(new HealthResponse(nameof(HealthStatus.Healthy))))
            .WithName("GetHealthLive")
            .Produces<HealthResponse>(StatusCodes.Status200OK);

        // Readiness: chạy mọi health check đã đăng ký (gồm DB); Unhealthy → 503
        group.MapGet("", async (HealthCheckService health, CancellationToken ct) =>
            {
                var report = await health.CheckHealthAsync(ct);
                var body = new HealthResponse(report.Status.ToString());
                return report.Status == HealthStatus.Unhealthy
                    ? Results.Json(body, statusCode: StatusCodes.Status503ServiceUnavailable)
                    : Results.Ok(body);
            })
            .WithName("GetHealth")
            .Produces<HealthResponse>(StatusCodes.Status200OK)
            .Produces<HealthResponse>(StatusCodes.Status503ServiceUnavailable);

        return api;
    }
}
