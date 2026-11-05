using HoangPhucSpa.Api.Infrastructure;
using HoangPhucSpa.Data.Entities;
using Microsoft.Extensions.Options;

namespace HoangPhucSpa.Tests;

public class PathsAndMediaUrlTests
{
    [Fact]
    public void Paths_Should_BuildEnglishRoutes_When_GivenSlugsAndIds()
    {
        Assert.Equal("/services/acne-treatment", Paths.Service("acne-treatment"));
        Assert.Equal("/services/12/back-acne-treatment", Paths.SubService(12, "back-acne-treatment"));
        Assert.Equal("/news/42/grand-opening", Paths.Post(PostKind.News, 42, "grand-opening"));
        Assert.Equal("/library/99/skincare-guide", Paths.Post(PostKind.Library, 99, "skincare-guide"));
        Assert.Equal("/news/categories/promotions", Paths.Category(PostKind.News, "promotions"));
        Assert.Equal("/library/categories/skincare-tips", Paths.Category(PostKind.Library, "skincare-tips"));
        Assert.Equal("/albums/customer-results", Paths.Album("customer-results"));
        Assert.Equal("/about-us", Paths.Page("about-us"));
    }

    [Fact]
    public void Absolute_Should_EscapePathSegments_When_RelativePathContainsSpacesOrSpecialCharacters()
    {
        var options = Options.Create(new AppOptions { PublicBaseUrl = "https://cdn.example.com/media/" });
        var builder = new MediaUrlBuilder(options);

        var url = builder.Absolute("uploads/2026/my photo #1.jpg");

        Assert.Equal("https://cdn.example.com/media/uploads/2026/my%20photo%20%231.jpg", url);
    }

    [Fact]
    public void Absolute_Should_ReturnNull_When_RelativePathIsNullOrWhiteSpace()
    {
        var options = Options.Create(new AppOptions { PublicBaseUrl = "https://cdn.example.com" });
        var builder = new MediaUrlBuilder(options);

        Assert.Null(builder.Absolute(null));
        Assert.Null(builder.Absolute("   "));
    }
}
