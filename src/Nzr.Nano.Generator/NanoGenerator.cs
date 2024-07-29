using System.Text.Json.Serialization;

namespace Nzr.Nano.Generator;

/// <summary>
/// Provides functionality to generate random character and placeholder sets for use with the Nano obfuscation library.
/// </summary>
public static class NanoGenerator
{
    /// <summary>
    /// Represents the configuration settings containing character and placeholder sets.
    /// </summary>
    public sealed class Settings
    {
        /// <summary>
        /// The Nano options containing the generated sets.
        /// </summary>
        [JsonPropertyName("NanoOptions")]
        public required NanoOptions NanoOptions { get; set; }
    }

    /// <summary>
    /// Generates a random set of character sets and placeholder sets in JSON format.
    /// </summary>
    /// <param name="allowedChars">
    /// A string of allowed characters to use for generation. Defaults to "0123456789BCDEFGHJKMNPQRSTUVWXYZ".
    /// This set excludes O, I, and L to avoid confusion with numbers and A to reduce the likelihood of offensive words.
    /// </param>
    /// <returns>
    /// A Settings container containing the generated character and placeholder sets.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="allowedChars"/> contains duplicates or does not meet minimum size requirements.
    /// </exception>
    public static Settings Generate(string allowedChars = "0123456789BCDEFGHJKMNPQRSTUVWXYZ")
    {
        const int characterSetsCount = 10; // Number of sets to generate
        const int characterPerSetsCount = 26; // Number of characters per character set

        ValidateAllowedChars(allowedChars, characterPerSetsCount);

        // Calculate the placeholder set length based on remaining characters
        var placeholderSetLength = allowedChars.Length - characterPerSetsCount;
        var random = new Random();

        var characterSets = new string[characterSetsCount];
        var placeholderSets = new string[characterSetsCount];

        for (var i = 0; i < characterSetsCount; i++)
        {
            // Generate a valid character set by shuffling the allowed characters
            var charSet = new string(allowedChars
                .OrderBy(_ => random.Next())
                .Take(characterPerSetsCount)
                .ToArray());

            // Determine the remaining characters for placeholders
            var remainingChars = allowedChars
                .Except(charSet)
                .ToArray();

            // Generate a valid placeholder set by shuffling the remaining characters
            var placeholderSet = new string(remainingChars
                .OrderBy(_ => random.Next())
                .Take(placeholderSetLength)
                .ToArray());

            // Assign the sets to their respective dictionaries
            characterSets[i] = charSet;
            placeholderSets[i] = placeholderSet;
        }

        // Validate the generated sets using Nano's configuration validation method
        Nano.ValidateConfigurations(characterSets, placeholderSets, "-");

        // Combine generated sets into the Settings object
        var settings = new Settings()
        {
            NanoOptions = new()
            {
                CharacterSets = characterSets,
                PlaceHolderCharSets = placeholderSets,
                NegativeSign = "-"
            }
        };

        return settings;
    }

    private static void ValidateAllowedChars(string allowedChars, int characterPerSetsCount)
    {
        // Validate the input character pool
        if (allowedChars.Distinct().ToArray().Length != allowedChars.Length)
        {
            throw new ArgumentException("Allowed characters must contain unique characters.");
        }
        else if (allowedChars.Length < characterPerSetsCount + 1) // +1 to ensure placeholders can be generated
        {
            throw new ArgumentException($"Allowed characters must contain at least {characterPerSetsCount + 1} unique characters.");
        }
    }
}
