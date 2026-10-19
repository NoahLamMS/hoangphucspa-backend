using System.ComponentModel.DataAnnotations;

namespace HoangPhucSpa.Api.Infrastructure;

public sealed class AppOptions
{
    [Required, Url]
    public string PublicBaseUrl { get; init; } = "";

    [Required, Url]
    public string AdminUrl { get; init; } = "";
}

public sealed class WebOptions
{
    [Required, Url]
    public string InternalBaseUrl { get; init; } = "";

    [Required, MinLength(32)]
    public string RevalidateSecret { get; init; } = "";
}

public sealed class MediaOptions
{
    public string RootPath { get; init; } = "";
    public long MaxUploadBytes { get; init; } = 10 * 1024 * 1024;
}
