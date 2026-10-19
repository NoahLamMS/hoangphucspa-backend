using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace HoangPhucSpa.Api.Infrastructure;

public static class DevMediaFiles
{
    public static WebApplication UseDevMediaFiles(this WebApplication app)
    {
        var mediaOptions = app.Services.GetService<IOptions<MediaOptions>>()?.Value;
        var root = mediaOptions?.RootPath;
        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root)) return app;

        foreach (var folder in new[] { "images", "media" })
        {
            var dir = Directory.CreateDirectory(Path.Combine(root, folder)).FullName;
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(dir),
                RequestPath = "/" + folder
            });
        }
        return app;
    }
}
