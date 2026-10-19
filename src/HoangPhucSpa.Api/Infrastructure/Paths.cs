using HoangPhucSpa.Data.Entities;

namespace HoangPhucSpa.Api.Infrastructure;

public static class Paths
{
    public static string Service(string slug) => $"/dich-vu/{slug}";

    public static string SubService(int id, string slug) => $"/dich-vu/{id}/{slug}";

    public static string Post(PostKind kind, int publicId, string slug) =>
        kind == PostKind.News ? $"/tin-tuc/{publicId}/{slug}" : $"/bai-dang/{publicId}/{slug}";

    public static string Category(PostKind kind, string slug) =>
        kind == PostKind.News ? $"/tin-tuc/danh-muc/{slug}" : $"/thu-vien/{slug}";

    public static string Page(string key) => key switch
    {
        "gioi-thieu" => "/gioi-thieu",
        "bang-gia" => "/bang-gia",
        "lam-kieu-spa-2" => "/lam-kieu-spa-2",
        _ => $"/{key}"
    };
}
