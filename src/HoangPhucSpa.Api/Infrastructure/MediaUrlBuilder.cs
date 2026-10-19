using Microsoft.Extensions.Options;

namespace HoangPhucSpa.Api.Infrastructure;

public sealed class MediaUrlBuilder(IOptions<AppOptions> options)
{
    private readonly string _baseUrl = options.Value.PublicBaseUrl.TrimEnd('/');

    public string? Absolute(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return null;
        if (relativePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            relativePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return relativePath;
        }

        var normalized = "/" + relativePath.TrimStart('/');
        return $"{_baseUrl}{normalized}";
    }
}
