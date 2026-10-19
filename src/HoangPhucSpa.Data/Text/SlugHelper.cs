using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HoangPhucSpa.Data.Text;

public static partial class SlugHelper
{
    public static string Generate(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        var s = RemoveDiacritics(text).ToLowerInvariant();
        s = NonAlnum().Replace(s, "-");
        return s.Trim('-');
    }

    public static bool IsValid(string slug) => ValidSlugRegex().IsMatch(slug);

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
