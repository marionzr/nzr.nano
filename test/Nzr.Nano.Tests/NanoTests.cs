using FluentAssertions;
using Snapshooter;
using Snapshooter.Xunit;

namespace Nzr.Nano.Tests;

/// <summary>
/// Tests for the <see cref="Nano"/> class, verifying its obfuscation and deobfuscation logic and configuration validation.
/// </summary>
public class NanoTests
{
    private readonly string[] _characterSets =
    [
        "H9MQB682DXPG0YKUTFS17CN54J",
        "TXMRD72S31Z6UEQ5K9B8F4NVHW",
        "Q8V4YC6ZGHFPEBMSU9KX05R13T",
        "F56SHTZC304Q2YRMPGU97EWXNB",
        "JYBUZG07C3DX912WV6FQNHT4ER",
        "MQ2KP9HF13ZYSJDTWRXV46EBU5",
        "W35NFPYTEQDJZBUHVG7CKX68S4",
        "59WMQKG68DS1PU7BTHZJVENR2C",
        "EPFNV85YG7XSJQWMKUT43C2Z9H",
        "SK825XNQY01VT7E4WDZHRM6FBC"
    ];

    private readonly string[] _placeholderSets =
    [
        "V3WERZ",
        "YPJ0GC",
        "J72NWD",
        "V1KD8J",
        "S5MK8P",
        "C70NG8",
        "R901M2",
        "3Y4F0X",
        "1R06BD",
        "3PJG9U"
    ];

    private const string NegativeSign = "-";

    public NanoTests()
    {
        Nano.Initialize(_characterSets, _placeholderSets, NegativeSign);
    }

    [Fact]
    public async Task Obfuscate_And_Deobfuscate_Should_Return_Original_Number_When_Ran_Concurrently()
    {
        // Arrange
        var random = new Random();
        var originalNumbers = new List<long>();

        for (var i = 0; i < 10_000; i++)
        {
            // Generate random long value
            var randomValue = (long)(random.NextDouble() * (double.MaxValue - long.MinValue)) + long.MinValue;
            originalNumbers.Add(randomValue);
        }

        // Act
        var tasks = originalNumbers.Select(originalNumber =>
        {
            return Task.Run(() =>
            {
                var obfuscatedNumber = Nano.Obfuscate(originalNumber, "banana");
                var deobfuscatedNumber = Nano.Deobfuscate(obfuscatedNumber, "banana");

                // Assert
                deobfuscatedNumber.Should().Be(originalNumber);
            });
        }).ToList();

        // Await all tasks asynchronously
        await Task.WhenAll(tasks); // No blocking operation, we are now awaiting the tasks properly
    }

    [Theory]
    [InlineData("a")]
    [InlineData("banana")]
    [InlineData("1")]
    [InlineData("12345")]
    [InlineData("7B725F60-9AFF-4C96-BE0F-914F4D0788DD")]
    public void Obfuscate_Deobfuscate_Should_Handle_Correctly(string key)
    {
        // Arrange

        long[] originalNumbers = {
            -9_223_372_036_854_775_807L, // The smallest supported number
            -9_223_372_036_854_775_806L,
            -2_147_483_649L, -2_147_483_648L, -2_147_483_647L,
            -32_769L, -32_768L, -32_767L,
            -1_001L, -1_000L, -999L,
            -101L, -100L, -99L,
            -12L, -11L, -10L,
            -11L, -10L, -9L,
            -3L, -2L, -1L,
            -2L, -1L, 0L,
            0L, 1L, 2L,
            1L, 2L, 3L,
            9L, 10L, 11L,
            10L, 11L, 12L,
            99L, 100L, 101L,
            999L, 1_000L, 1_001L,
            32_766L, 32_767L, 32_768L,
            2_147_483_646L, 2_147_483_647L, 2_147_483_648L,
            9_223_372_036_854_775_807L // The largest supported number
        };

        // Act

        for (var i = 0; i <= _placeholderSets[0].Length; i++)
        {
            var obfuscatedNumbers = new HashSet<string>(originalNumbers.Length);

            foreach (var originalNumber in originalNumbers)
            {
                var obfuscatedNumber = Nano.Obfuscate(originalNumber, key, i);
                var deobfuscatedNumber = Nano.Deobfuscate(obfuscatedNumber, key);

                // Assert

                deobfuscatedNumber.Should().Be(originalNumber);
                obfuscatedNumbers.Add(obfuscatedNumber);
            }

            // Assert

            obfuscatedNumbers.Should().MatchSnapshot(SnapshotNameExtension.Create($"with_key_{key}_with_minLen_{i}"));
        }
    }

    [Fact]
    public void Obfuscate_Should_Handle_Negative_Numbers_Correctly()
    {
        // Arrange

        long originalNumber = -12345;
        var key = "Banana";

        // Act

        var obfuscated = Nano.Obfuscate(originalNumber, key);
        var deobfuscated = Nano.Deobfuscate(obfuscated, key);

        // Assert

        deobfuscated.Should().Be(originalNumber);
        obfuscated.Should().StartWith(NegativeSign);
    }

    [Fact]
    public void Obfuscate_Should_Add_Placeholders_For_Short_Encoded_Values()
    {
        // Arrange

        var originalNumber = 1L;
        var key = Guid.NewGuid().ToString();
        var minLength = 6;

        // Act

        var obfuscated = Nano.Obfuscate(originalNumber, key, minLength);

        // Assert

        obfuscated.Length.Should().BeGreaterOrEqualTo(minLength);
    }

    [Fact]
    public void Deobfuscate_Should_Remove_Placeholders_Correctly()
    {
        // Arrange

        var originalNumber = 12345L;
        var key = new Random(Guid.NewGuid().GetHashCode()).NextDouble().ToString();

        // Act

        var obfuscated = Nano.Obfuscate(originalNumber, key);
        var deobfuscated = Nano.Deobfuscate(obfuscated, key);

        // Assert

        deobfuscated.Should().Be(originalNumber);
    }

    [Fact]
    public void Deobfuscate_Should_Throw_When_Value_Contains_Invalid_Characters()
    {
        // Arrange

        var invalidValue = "A1B2#C3"; // Contains invalid character '#'
        var key = "Banana";

        // Act

        var act = () => Nano.Deobfuscate(invalidValue, key);

        // Assert

        act.Should().Throw<ArgumentException>()
            .WithMessage("The value 'A1B2#C3' is not a valid obfuscated number for the provided obfuscation key.");
    }


    [Fact]
    public void Validate_Should_Throw_When_Placeholder_Overlaps_With_CharacterSet()
    {
        // Arrange

        var invalidPlaceholderSets = _characterSets;
        invalidPlaceholderSets[0] = "012"; // Overlaps with character set

        // Act

        var act = () => Nano.ValidateConfigurations(_characterSets, invalidPlaceholderSets, NegativeSign);

        // Assert

        act.Should().Throw<ArgumentException>()
            .WithMessage("Character set and placeholder set at index 0 contain overlapping characters.");
    }

    [Fact]
    public void Validate_Should_Throw_When_Negative_Sign_Is_Invalid()
    {
        // Arrange

        var invalidNegativeSign = "--";

        // Act

        var act = () => Nano.ValidateConfigurations(_characterSets, _placeholderSets, invalidNegativeSign);

        // Assert

        act.Should().Throw<ArgumentException>()
            .WithMessage($"Negative sign must be a single character, but '{invalidNegativeSign}' was provided.");
    }

    [Theory]
    [InlineData("-")]
    [InlineData("#")]
    public void Validate_Should_Throw_When_Negative_Sign_Exists_In_CharacterSets(string negativeSign)
    {
        // Arrange

        var invalidPlaceholderSets = _characterSets;
        invalidPlaceholderSets[0] = "0123456789BCDEFGHJKMNPQRSTUVXWYZ" + negativeSign;

        // Act

        var act = () => Nano.ValidateConfigurations(invalidPlaceholderSets, _placeholderSets, negativeSign);

        // Assert

        act.Should().Throw<ArgumentException>()
            .WithMessage($"Negative sign '{negativeSign}' cannot be present in any character set or placeholder character set.");
    }
}
