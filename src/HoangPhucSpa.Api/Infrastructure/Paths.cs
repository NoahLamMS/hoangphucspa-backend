using HoangPhucSpa.Data.Entities;

namespace HoangPhucSpa.Api.Infrastructure;

/// <summary>
/// Tập trung toàn bộ quy tắc tạo đường dẫn URL chuẩn tiếng Anh cho Frontend (phục vụ SEO, Sitemap, Redirects).
/// </summary>
public static class Paths
{
    public static string Service(string slug) => $"/services/{slug}";

    public static string SubService(int id, string slug) => $"/services/{id}/{slug}";

    public static string Post(PostKind kind, int id, string slug) =>
        kind == PostKind.News ? $"/news/{id}/{slug}" : $"/library/{id}/{slug}";

    public static string Category(PostKind kind, string slug) =>
        kind == PostKind.News ? $"/news/categories/{slug}" : $"/library/categories/{slug}";

    public static string Album(string slug) => $"/albums/{slug}";

    public static string Page(string slug) => $"/{slug.TrimStart('/')}";
}
