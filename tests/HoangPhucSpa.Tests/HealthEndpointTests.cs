using System.Net;
using System.Net.Http.Json;
using HoangPhucSpa.Api.Features.Health;

namespace HoangPhucSpa.Tests;

public class HealthEndpointTests
{
    [Fact]
    public async Task GetLive_Should_Return200Healthy_When_ApplicationIsRunning()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("Healthy", body?.Status);
    }

    [Fact]
    public async Task GetReady_Should_Return503Unhealthy_When_DatabaseIsUnreachable()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("Unhealthy", body?.Status);
    }

    [PostgresFact]
    public async Task GetReady_Should_Return200Healthy_When_DatabaseIsReachable()
    {
        var connectionString = Environment.GetEnvironmentVariable(PostgresFactAttribute.ConnectionStringVariable);
        await using var factory = new ApiFactory(connectionString);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("Healthy", body?.Status);
    }

    [Fact]
    public async Task OpenApi_Should_Return200WithSchema_When_EnvironmentIsDevelopment()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("HealthResponse", json);
    }
}
