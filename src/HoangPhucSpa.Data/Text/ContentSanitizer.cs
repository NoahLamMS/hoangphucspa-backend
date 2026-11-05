using Ganss.Xss;

namespace HoangPhucSpa.Data.Text;

/// <summary>
/// Làm sạch mã HTML trước khi lưu vào DB nhằm chống tấn công XSS nhưng vẫn giữ lại thẻ định dạng bài viết và iframe hợp lệ (YouTube, Google Maps).
/// </summary>
public static class ContentSanitizer
{
    public static HtmlSanitizer Create()
    {
        var s = new HtmlSanitizer();
        s.AllowedTags.UnionWith(["iframe", "figure", "figcaption"]);
        s.AllowedAttributes.UnionWith(["class", "target", "rel", "loading", "allow", "allowfullscreen", "frameborder", "title"]);
        s.AllowedSchemes.UnionWith(["mailto", "tel"]);
        s.PostProcessDom += (_, e) =>
        {
            foreach (var f in e.Document.QuerySelectorAll("iframe").ToList())
            {
                if (!IsAllowedIframe(f.GetAttribute("src")))
                {
                    f.Remove();
                    continue;
                }
                f.SetAttribute("loading", "lazy");
                if (string.IsNullOrEmpty(f.GetAttribute("title")))
                {
                    f.SetAttribute("title", "Video");
                }
            }
            foreach (var a in e.Document.QuerySelectorAll("a[target=_blank]"))
            {
                a.SetAttribute("rel", "noopener noreferrer");
            }
        };
        return s;
    }

    private static bool IsAllowedIframe(string? src) =>
        Uri.TryCreate(src, UriKind.Absolute, out var u) && u.Scheme == Uri.UriSchemeHttps &&
        (u.Host is "www.youtube.com" or "youtube.com" or "www.youtube-nocookie.com"
         || (u.Host == "www.google.com" && u.AbsolutePath.StartsWith("/maps/embed")));
}
