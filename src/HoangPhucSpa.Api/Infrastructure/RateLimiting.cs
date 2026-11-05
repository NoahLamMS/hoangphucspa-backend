using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace HoangPhucSpa.Api.Infrastructure;

/// <summary>
/// Định nghĩa tên các chính sách giới hạn tần suất gọi API (Rate Limiting) theo từng nhóm chức năng.
/// </summary>
public static class RateLimitPolicies
{
    public const string Public = "public";
    public const string Search = "search";
    public const string Views = "views";
    public const string Login = "login";
    public const string ForgotPassword = "forgot";
}

/// <summary>
/// Cấu hình giới hạn số lượng request theo địa chỉ IP của client (miễn trừ cho mạng nội bộ / container SSR).
/// </summary>
public static class RateLimiting
{
    private const string DefaultTrustedNetwork = "172.30.0.0/24";
    private const int PublicPermitLimit = 300;
    private const int SearchPermitLimit = 60;
    private const int ViewsPermitLimit = 30;
    private const int LoginPermitLimit = 5;
    private const int ForgotPasswordPermitLimit = 3;

    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services) => services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = (ctx, _) =>
        {
            if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
            {
                ctx.HttpContext.Response.Headers.RetryAfter = ((int)retry.TotalSeconds).ToString();
            }
            return ValueTask.CompletedTask;
        };

        options.AddPolicy(RateLimitPolicies.Public, ctx => PerIp(ctx, PublicPermitLimit, TimeSpan.FromMinutes(1)));
        options.AddPolicy(RateLimitPolicies.Search, ctx => PerIp(ctx, SearchPermitLimit, TimeSpan.FromMinutes(1)));
        options.AddPolicy(RateLimitPolicies.Views, ctx => PerIp(ctx, ViewsPermitLimit, TimeSpan.FromMinutes(1)));
        options.AddPolicy(RateLimitPolicies.Login, ctx => PerIp(ctx, LoginPermitLimit, TimeSpan.FromMinutes(1)));
        options.AddPolicy(RateLimitPolicies.ForgotPassword, ctx => PerIp(ctx, ForgotPasswordPermitLimit, TimeSpan.FromHours(1)));
    });

    private static RateLimitPartition<string> PerIp(HttpContext ctx, int limit, TimeSpan window)
    {
        var ip = ctx.Connection.RemoteIpAddress;
        if (ip is null || IsInternal(ctx, ip))
        {
            return RateLimitPartition.GetNoLimiter("internal");
        }

        return RateLimitPartition.GetFixedWindowLimiter(
            ip.ToString(),
            _ => new FixedWindowRateLimiterOptions { PermitLimit = limit, Window = window, QueueLimit = 0 });
    }

    private static bool IsInternal(HttpContext ctx, IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip))
        {
            return true;
        }

        var trusted = ctx.RequestServices.GetRequiredService<IConfiguration>()["Proxy:TrustedNetwork"];
        return IPNetwork.TryParse(trusted ?? DefaultTrustedNetwork, out var net) && net.Contains(ip);
    }
}
