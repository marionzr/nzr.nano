using System.Text.Json.Serialization;

namespace Nzr.Nano;

/// <summary>
/// Represents the configuration options for the Nano obfuscator.
/// </summary>
public record NanoOptions
{
    /// <summary>
    /// The array of character sets used for encoding and decoding numbers.
    /// Each character set must be unique and provide sufficient characters for obfuscation.
    /// </summary>
    /// <remarks>
    /// - Character sets must have the same length as the placeholder character sets.
    /// - Each character in a character set should be unique and not overlap with characters in the corresponding placeholder set.
    /// </remarks>
    [JsonPropertyName("CharacterSets")]
    public required string[] CharacterSets { get; set; }

    /// <summary>
    /// The array of placeholder character sets used for padding obfuscated strings to meet minimum length requirements.
    /// </summary>
    /// <remarks>
    /// - Placeholder character sets must have the same length as the character sets.
    /// - Characters in a placeholder set must not overlap with characters in the corresponding character set.
    /// </remarks>
    [JsonPropertyName("PlaceHolderCharSets")]
    public required string[] PlaceHolderCharSets { get; set; }

    /// <summary>
    /// The character to use as a negative sign for obfuscating negative numbers.
    /// </summary>
    /// <remarks>
    /// - The negative sign must be a single character.
    /// - The negative sign must not be present in any character set or placeholder character set.
    /// - If not explicitly specified, the default negative sign is a hyphen ("-").
    /// </remarks>
    [JsonPropertyName("NegativeSign")]
    public string? NegativeSign { get; set; }
}
