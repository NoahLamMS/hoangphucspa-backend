using Microsoft.Extensions.Options;

namespace HoangPhucSpa.Api.Infrastructure;

/// <summary>
/// Chuyển đổi đường dẫn tương đối của file ảnh/media trong DB thành URL tuyệt đối trả về cho Frontend.
/// </summary>
public sealed class MediaUrlBuilder(IOptions<AppOptions> options)
{
    private readonly string _baseUrl = options.Value.PublicBaseUrl.TrimEnd('/');

    public string? Absolute(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        if (relativePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            relativePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return relativePath;
        }

        // Mã hóa từng phần của đường dẫn để tránh lỗi URL khi tên file chứa khoảng trắng hoặc ký tự đặc biệt.
        var segments = relativePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(Uri.EscapeDataString);

        return $"{_baseUrl}/{string.Join('/', segments)}";
    }
}
