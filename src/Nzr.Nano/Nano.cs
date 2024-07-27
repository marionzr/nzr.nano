using System.Text;

namespace Nzr.Nano;

/// <summary>
/// Provides functionality for obfuscating and deobfuscating numbers using configurable character sets and placeholders.
/// </summary>
public static class Nano
{
    /// <summary>
    /// Gets a value indicating whether Nano has been initialized.
    /// </summary>
    public static bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// Array of character sets used for encoding and decoding numbers.
    /// </summary>
    private static string[] _characterSets = null!;

    /// <summary>
    /// Array of placeholder character sets used for padding obfuscated strings to meet minimum length requirements.
    /// </summary>
    private static string[] _placeHolderCharSets = null!;

    /// <summary>
    /// Character used to denote negative numbers during obfuscation.
    /// </summary>
    private static string _negativeSign = null!;

    private static readonly object _lock = new();

    /// <summary>
    /// Initializes Nano with an instance of <see cref="NanoOptions"/>.
    /// </summary>
    /// <param name="options">The configuration options for Nano.</param>
    public static void Initialize(NanoOptions options)
    {
        Initialize(options.CharacterSets, options.PlaceHolderCharSets, options.NegativeSign ?? "-");
    }

    /// <summary>
    /// Initializes Nano with explicit configuration values.
    /// </summary>
    /// <param name="characterSets">An array of character sets for encoding/decoding.</param>
    /// <param name="placeHolderCharSets">An array of placeholder character sets for padding.</param>
    /// <param name="negativeSign">The character used to indicate negative numbers.</param>
    public static void Initialize(string[] characterSets, string[] placeHolderCharSets, string negativeSign)
    {
        lock (_lock)
        {
            ValidateConfigurations(characterSets, placeHolderCharSets, negativeSign);

            _characterSets = characterSets;
            _placeHolderCharSets = placeHolderCharSets;
            _negativeSign = string.IsNullOrWhiteSpace(negativeSign) ? "-" : negativeSign;

            IsInitialized = true;
        }
    }

    private static void EnsureInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("Nano has not been initialized. Please initialize Nano using NanoFactory or an appropriate Initialize method.");
        }
    }

    /// <summary>
    /// Obfuscates a number into a string using the specified obfuscation key.
    /// </summary>
    /// <param name="number">The number to obfuscate.</param>
    /// <param name="obfuscationKey">The key used to determine the character set for obfuscation.</param>
    /// <param name="minLength">The minimum length of the obfuscated string (default is 6).</param>
    /// <returns>A string representing the obfuscated number.</returns>
    /// <exception cref="ArgumentException">Thrown if the number is <see cref="long.MinValue"/> or the minimum length exceeds the available placeholder character sets.</exception>
    public static string Obfuscate(long number, string obfuscationKey, int minLength = 6)
    {
        EnsureInitialized();

        if (number == long.MinValue)
        {
            throw new ArgumentException($"The number '{number}' is not supported for obfuscation due to its absolute value exceeding supported ranges.");
        }

        if (minLength > _placeHolderCharSets.Length)
        {
            throw new ArgumentException($"The minimum length of the obfuscated string cannot exceed the length of the placeholder character sets.");
        }

        (var characterSet, var placeHolderCharSet) = GetCharacterSets(obfuscationKey);

        var isNegative = number < 0;
        number = Math.Abs(number);

        var encoded = EncodeNumber(number, characterSet);

        if (encoded.Length < minLength)
        {
            encoded = AddPlaceholderCharacters(encoded, number.GetHashCode(), minLength, placeHolderCharSet);
        }

        return isNegative ? _negativeSign + encoded : encoded;
    }

    /// <summary>
    /// Reverses the obfuscation process to retrieve the original number.
    /// </summary>
    /// <param name="value">The obfuscated string to decode.</param>
    /// <param name="obfuscationKey">The key used for obfuscation.</param>
    /// <returns>The original number.</returns>
    /// <exception cref="ArgumentException">Thrown if the value contains invalid characters or is improperly formatted.</exception>
    public static long Deobfuscate(string value, string obfuscationKey)
    {
        EnsureInitialized();

        (var characterSet, var placeHolderCharSet) = GetCharacterSets(obfuscationKey);

        var validCharacters = new HashSet<char>(characterSet.Concat(placeHolderCharSet).Append(_negativeSign[0]));

        if (string.IsNullOrWhiteSpace(value) || value.Any(c => !validCharacters.Contains(c)))
        {
            throw new ArgumentException($"The value '{value}' is not a valid obfuscated number for the provided obfuscation key.");
        }

        var isNegative = value.StartsWith(_negativeSign);

        if (isNegative)
        {
            value = value[_negativeSign.Length..];
        }

        var cleanedValue = new string(value.Where(c => !placeHolderCharSet.Contains(c)).ToArray());

        var number = DecodeString(cleanedValue, characterSet);

        return isNegative ? -number : number;
    }

    private static (string characterSet, string placeHolderCharSet) GetCharacterSets(string obfuscationKey)
    {
        var index = Math.Abs(GetStableHashCode(obfuscationKey)) % _characterSets.Length;
        return (_characterSets[index], _placeHolderCharSets[index]);
    }

    private static string EncodeNumber(long number, string characterSet)
    {
        var baseLength = characterSet.Length;
        var sb = new StringBuilder();

        do
        {
            sb.Insert(0, characterSet[(int)(number % baseLength)]);
            number /= baseLength;
        } while (number > 0);

        return sb.ToString();
    }

    private static long DecodeString(string value, string characterSet)
    {
        var baseLength = characterSet.Length;
        long result = 0;

        foreach (var c in value)
        {
            result = (result * baseLength) + characterSet.IndexOf(c);
        }

        return result;
    }

    /// <summary>
    /// Adds placeholder characters to a string to meet a minimum length.
    /// </summary>
    /// <param name="sequence">The string to pad with placeholders.</param>
    /// <param name="seed">A seed value used to randomize placeholder insertion.</param>
    /// <param name="minLength">The minimum length the string should have.</param>
    /// <param name="placeHolderCharSet">The placeholder character set to use.</param>
    /// <returns>The padded string.</returns>

    private static string AddPlaceholderCharacters(string sequence, int seed, int minLength, string placeHolderCharSet)
    {
        if (string.IsNullOrEmpty(sequence))
        {
            return sequence;
        }

        var sb = new StringBuilder(sequence);

        while (sb.Length < minLength)
        {
            var hash = Math.Abs(GetStableHashCode(sb.ToString()));
            var index = (hash ^ seed) % placeHolderCharSet.Length;
            sb.Append(placeHolderCharSet[index]);
        }

        return sb.ToString();
    }

    public static void ValidateConfigurations(string[] characterSets, string[] placeHolderCharSets, string negativeSign)
    {
        ArgumentNullException.ThrowIfNull(characterSets);
        ArgumentNullException.ThrowIfNull(placeHolderCharSets);

        const int minCharacterSetLength = 26;

        if (negativeSign.Length != 1)
        {
            throw new ArgumentException($"Negative sign must be a single character, but '{negativeSign}' was provided.");
        }

        if (characterSets.Any(set => set.Contains(negativeSign)) ||
            placeHolderCharSets.Any(set => set.Contains(negativeSign)))
        {
            throw new ArgumentException($"Negative sign '{negativeSign}' cannot be present in any character set or placeholder character set.");
        }

        if (characterSets.Length != placeHolderCharSets.Length)
        {
            throw new ArgumentException("Character sets and placeholder sets must have the same length.");
        }

        for (var i = 0; i < characterSets.Length; i++)
        {
            if (characterSets[i].Intersect(placeHolderCharSets[i]).Any())
            {
                throw new ArgumentException($"Character set and placeholder set at index {i} contain overlapping characters.");
            }

            if (characterSets[i].Length < minCharacterSetLength)
            {
                throw new ArgumentException($"Character set at index {i} must contain at least {minCharacterSetLength} characters.");
            }
        }
    }

    private static int GetStableHashCode(string key)
    {
        unchecked
        {
            var hash = 23;
            foreach (var c in key)
            {
                hash = (hash * 31) + c;
            }
            return hash;
        }
    }
}
