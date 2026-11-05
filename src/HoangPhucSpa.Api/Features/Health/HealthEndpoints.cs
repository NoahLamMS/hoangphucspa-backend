using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HoangPhucSpa.Api.Features.Health;

public sealed record HealthResponse(string Status);

/// <summary>
/// Nhóm endpoint kiểm tra trạng thái sống (Liveness) và sẵn sàng kết nối DB (Readiness) của hệ thống.
/// </summary>
public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("/health").WithTags("Health");

        // Liveness probe: chỉ kiểm tra tiến trình API đang chạy, không gọi DB.
        group.MapGet("/live", GetLiveness)
            .WithName("GetHealthLive")
            .WithSummary("Check application liveness without database connectivity");

        // Readiness probe: kiểm tra kết nối thực tế tới PostgreSQL thông qua HealthCheckService.
        group.MapGet("", GetReadiness)
            .WithName("GetHealth")
            .WithSummary("Check application readiness including database connectivity")
            .Produces<HealthResponse>(StatusCodes.Status200OK)
            .Produces<HealthResponse>(StatusCodes.Status503ServiceUnavailable);

        return api;
    }

    private static Ok<HealthResponse> GetLiveness() =>
        TypedResults.Ok(new HealthResponse(nameof(HealthStatus.Healthy)));

    private static async Task<Results<Ok<HealthResponse>, JsonHttpResult<HealthResponse>>> GetReadiness(
        HealthCheckService health,
        CancellationToken ct)
    {
        var report = await health.CheckHealthAsync(ct);
        var body = new HealthResponse(report.Status.ToString());

        return report.Status == HealthStatus.Unhealthy
            ? TypedResults.Json(body, statusCode: StatusCodes.Status503ServiceUnavailable)
            : TypedResults.Ok(body);
    }
}
