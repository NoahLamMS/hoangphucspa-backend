using System.Net;
using System.Net.Http.Json;
using HoangPhucSpa.Api.Features.Health;

namespace HoangPhucSpa.Tests;

public class HealthEndpointTests
{
    [Fact]
    public async Task Live_returns_200_Healthy()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("Healthy", body?.Status);
    }

    [Fact]
    public async Task Health_returns_503_Unhealthy_when_db_unreachable()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("Unhealthy", body?.Status);
    }

    [SqlServerFact]
    public async Task Health_returns_200_Healthy_with_real_db()
    {
        var password = Environment.GetEnvironmentVariable(SqlServerFactAttribute.PasswordVariable);
        // Database=master luôn tồn tại, nên test không phụ thuộc DB HoangPhucSpa đã được tạo hay chưa.
        var connectionString =
            $"Server=localhost,1433;Database=master;User Id=sa;Password={password};TrustServerCertificate=True;Connect Timeout=5";
        await using var factory = new ApiFactory(connectionString);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("Healthy", body?.Status);
    }

    [Fact]
    public async Task OpenApi_is_served_in_Development()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("HealthResponse", json);
    }
}
