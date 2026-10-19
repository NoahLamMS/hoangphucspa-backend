using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace HoangPhucSpa.Api.Infrastructure;

public static class RateLimiting
{
    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services) => services.AddRateLimiter(o =>
    {
        o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        o.OnRejected = (ctx, _) =>
        {
            if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
            {
                ctx.HttpContext.Response.Headers.RetryAfter = ((int)retry.TotalSeconds).ToString();
            }
            return ValueTask.CompletedTask;
        };
        o.AddPolicy("public", ctx => PerIp(ctx, 300, TimeSpan.FromMinutes(1)));
        o.AddPolicy("search", ctx => PerIp(ctx, 60, TimeSpan.FromMinutes(1)));
        o.AddPolicy("views", ctx => PerIp(ctx, 30, TimeSpan.FromMinutes(1)));
        o.AddPolicy("login", ctx => PerIp(ctx, 5, TimeSpan.FromMinutes(1)));
        o.AddPolicy("forgot", ctx => PerIp(ctx, 3, TimeSpan.FromHours(1)));
    });

    private static RateLimitPartition<string> PerIp(HttpContext ctx, int limit, TimeSpan window)
    {
        var ip = ctx.Connection.RemoteIpAddress;
        if (ip is null || IsInternal(ctx, ip))
        {
            return RateLimitPartition.GetNoLimiter("internal");
        }
        return RateLimitPartition.GetFixedWindowLimiter(ip.ToString(),
            _ => new FixedWindowRateLimiterOptions { PermitLimit = limit, Window = window, QueueLimit = 0 });
    }

    private static bool IsInternal(HttpContext ctx, IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip)) return true;
        var trusted = ctx.RequestServices.GetRequiredService<IConfiguration>()["Proxy:TrustedNetwork"];
        if (IPNetwork.TryParse(trusted ?? "172.30.0.0/24", out var net))
        {
            return net.Contains(ip);
        }
        return false;
    }
}
