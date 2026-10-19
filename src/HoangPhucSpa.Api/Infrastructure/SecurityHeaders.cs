namespace HoangPhucSpa.Api.Infrastructure;

public static class SecurityHeaders
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app) => app.Use(async (ctx, next) =>
    {
        ctx.Response.OnStarting(() =>
        {
            var h = ctx.Response.Headers;
            h.XContentTypeOptions = "nosniff";
            h.XFrameOptions = "DENY";
            h["Referrer-Policy"] = "strict-origin-when-cross-origin";
            var path = ctx.Request.Path;
            if (path.StartsWithSegments("/api/v1/admin") || path.StartsWithSegments("/api/v1/auth"))
            {
                h.CacheControl = "no-store";
            }
            return Task.CompletedTask;
        });
        await next();
    });
}
