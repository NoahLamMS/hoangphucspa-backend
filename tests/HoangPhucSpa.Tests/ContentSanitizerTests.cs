using HoangPhucSpa.Data.Text;

namespace HoangPhucSpa.Tests;

public class ContentSanitizerTests
{
    private readonly Ganss.Xss.HtmlSanitizer _sanitizer = ContentSanitizer.Create();

    [Theory]
    [InlineData("<script>alert(1)</script>", "")]
    [InlineData("<img src=x onerror=alert(1)>", "<img src=\"x\">")]
    [InlineData("<a href=\"javascript:alert(1)\">click</a>", "<a>click</a>")]
    [InlineData("<iframe src=\"https://evil.example.com\"></iframe>", "")]
    public void Sanitize_Should_RemoveMaliciousContent_When_GivenUnsafeHtml(string input, string expected)
    {
        var result = _sanitizer.Sanitize(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Sanitize_Should_PreserveIframe_When_SourceIsAllowedYoutubeHost()
    {
        var input = "<iframe src=\"https://www.youtube.com/embed/xyz\"></iframe>";
        var result = _sanitizer.Sanitize(input);
        Assert.Contains("youtube.com", result);
        Assert.Contains("loading=\"lazy\"", result);
    }
}
