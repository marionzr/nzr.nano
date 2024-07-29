using System.Text.Json;
using FluentAssertions;

namespace Nzr.Nano.Generator.Tests;

public class NanoGeneratorTests
{
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    /// <summary>
    /// Validates that the generated JSON string matches the serialized settings object.
    /// </summary>
    [Fact]
    public void Generate_Should_Generate_Valid_Json_Structure_When_Called_With_Default_Settings()
    {
        // Act

        var settings = NanoGenerator.Generate();

        // Assert

        var settingsJson = JsonSerializer.Serialize(settings, _options);

        settingsJson.Should().ContainAll("nanoOptions", "characterSets", "placeHolderCharSets", "negativeSign");
    }

    /// <summary>
    /// Validates that custom allowed characters are supported during generation.
    /// </summary>
    [Theory]
    [InlineData("0123456789AaBbCcDdEeFfGgHhIiJjLlMmNnOoPpRrSsQqUuVvWwXxYyZz")]
    [InlineData("0123456789\u00D6\u00DC\u00C4\u00DF\u00C9\u00CA\u00C1\u00CD\u00CC\u00D3\u00D2#@$%&?!")]
    public void Generate_Should_Allow_Custom_Allowed_Chars_When_Custom_Chars_Provided(string allowedChars)
    {
        // Act

        var settings = NanoGenerator.Generate(allowedChars);

        // Assert

        allowedChars.All(c => settings.NanoOptions.CharacterSets.Any(s => s.Contains(c)) ||
                              settings.NanoOptions.PlaceHolderCharSets.Any(s => s.Contains(c))).Should().BeTrue();
    }

    /// <summary>
    /// Ensures that all character sets generated have unique characters with no duplicates.
    /// </summary>
    [Fact]
    public void Generate_Should_Produce_Unique_Character_Sets_When_Generated()
    {
        // Act

        var settings = NanoGenerator.Generate();

        // Assert

        var characterSets = settings.NanoOptions.CharacterSets;

        foreach (var set in characterSets)
        {
            set.Distinct().Should().HaveCount(set.Length, "Character sets should not have duplicate characters.");
        }
    }

    /// <summary>
    /// Ensures that placeholder character sets generated have unique characters with no duplicates.
    /// </summary>
    [Fact]
    public void Generate_Should_Produce_Unique_Placeholder_CharacterSets_When_Generated()
    {
        // Act

        var settings = NanoGenerator.Generate();

        // Assert

        var placeHolderCharSets = settings.NanoOptions.PlaceHolderCharSets;

        foreach (var set in placeHolderCharSets)
        {
            set.Distinct().Should().HaveCount(set.Length, "Placeholder character sets should not have duplicate characters.");
        }
    }

    /// <summary>
    /// Validates that placeholder sets do not overlap with character sets for each key.
    /// </summary>
    [Fact]
    public void Generate_Should_Ensure_PlaceholderSets_Do_Not_Overlap_With_CharacterSets_When_Generated()
    {
        // Act

        var settings = NanoGenerator.Generate();

        // Assert

        var characterSets = settings.NanoOptions.CharacterSets;
        var placeholderSets = settings.NanoOptions.PlaceHolderCharSets;

        for (var i = 0; i < characterSets.Length; i++)
        {
            var charSet = characterSets[i];
            var placeholderSet = placeholderSets[i];

            charSet.Intersect(placeholderSet).Should().BeEmpty($"Placeholder set for index '{i}' overlaps with character set.");
        }
    }

    /// <summary>
    /// Validates that an exception is thrown when duplicate characters are provided in allowedChars.
    /// </summary>
    [Fact]
    public void Generate_Should_Throw_Exception_When_Duplicated_Chars_Are_Detected()
    {
        // Arrange

        var invalidChars = "0123456789BCDEFF"; // F is duplicated

        // Act

        var act = () => NanoGenerator.Generate(invalidChars);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Allowed characters must contain unique characters.");
    }

    /// <summary>
    /// Validates that an exception is thrown when allowed characters are fewer than the required minimum.
    /// </summary>
    [Fact]
    public void Generate_Should_Throw_Exception_When_Allowed_Chars_Are_Less_Than_Minimum()
    {
        // Arrange

        var invalidChars = "0123456789ABCDEF"; // Less than 27 unique characters

        // Act

        var act = () => NanoGenerator.Generate(invalidChars);

        // Assert

        act.Should().Throw<ArgumentException>()
            .WithMessage("Allowed characters must contain at least 27 unique characters.");
    }
}
