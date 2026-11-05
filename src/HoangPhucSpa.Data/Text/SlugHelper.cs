using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HoangPhucSpa.Data.Text;

/// <summary>
/// Tiện ích xử lý chuỗi tiếng Việt: tạo đường dẫn tĩnh (slug), kiểm tra tính hợp lệ của slug và tạo chuỗi tìm kiếm không dấu.
/// </summary>
public static partial class SlugHelper
{
    /// <summary>
    /// Chuyển tiêu đề tiếng Việt có dấu thành slug chuẩn (chữ thường, không dấu, nối bằng dấu gạch ngang).
    /// </summary>
    public static string Generate(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        var s = RemoveDiacritics(text).ToLowerInvariant();
        s = NonAlnum().Replace(s, "-");
        return s.Trim('-');
    }

    /// <summary>
    /// Kiểm tra chuỗi có đúng định dạng slug chuẩn (chỉ gồm a-z, 0-9 và dấu '-' ở giữa) hay không.
    /// </summary>
    public static bool IsValid(string slug) => ValidSlugRegex().IsMatch(slug);

    /// <summary>
    /// Gộp nhiều đoạn văn bản và chuyển về dạng chữ thường không dấu để phục vụ tìm kiếm toàn văn (SearchText).
    /// </summary>
    public static string ToSearchText(params string?[] parts)
    {
        var joined = string.Join(' ', parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        return MultiSpace().Replace(NonAlnum().Replace(RemoveDiacritics(joined).ToLowerInvariant(), " "), " ").Trim();
    }

    private static string RemoveDiacritics(string? text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        var sb = new StringBuilder(text.Length);
        foreach (var ch in text.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }
        return sb.ToString().Replace('\u0111', 'd').Replace('\u0110', 'D').Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex("[^a-z0-9]+")] private static partial Regex NonAlnum();
    [GeneratedRegex(" {2,}")] private static partial Regex MultiSpace();
    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")] private static partial Regex ValidSlugRegex();
}
