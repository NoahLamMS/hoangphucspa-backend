using System.Text.Json;
using HoangPhucSpa.Data.Text;

namespace HoangPhucSpa.Tests;

public class SlugHelperTests
{
    private sealed record GoldenCase(string Input, string Generate);

    [Fact]
    public void Generate_Should_MatchGoldenCases_When_GivenGoldenDataset()
    {
        var goldenPath = Path.Combine(AppContext.BaseDirectory, "golden", "slug-golden.json");
        var json = File.ReadAllText(goldenPath);
        var cases = JsonSerializer.Deserialize<List<GoldenCase>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        foreach (var c in cases)
        {
            var result = SlugHelper.Generate(c.Input);
            Assert.Equal(c.Generate, result);
        }
    }

    [Fact]
    public void ToSearchText_Should_NormalizeDiacriticsAndSpaces_When_GivenMultipleStrings()
    {
        var result = SlugHelper.ToSearchText("Trị Mụn  LƯNG", "và", "VẾT THÂM");
        Assert.Equal("tri mun lung va vet tham", result);
    }

    [Theory]
    [InlineData("tri-mun", true)]
    [InlineData("cham-soc-da-123", true)]
    [InlineData("-tri-mun", false)]
    [InlineData("tri--mun", false)]
    [InlineData("tri mun", false)]
    [InlineData("", false)]
    public void IsValid_Should_ReturnExpectedResult_When_GivenCandidateSlug(string slug, bool expected)
    {
        Assert.Equal(expected, SlugHelper.IsValid(slug));
    }
}
