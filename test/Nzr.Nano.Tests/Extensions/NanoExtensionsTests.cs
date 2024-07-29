using FluentAssertions;
using Nzr.Nano.Extensions;

namespace Nzr.Nano.Tests.Extensions;

public class NanoExtensionsTests
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


    private const string TestKey1 = "KEY01";
    private const string TestKey2 = "KEY02";
    private const string TestKey3 = "KEY03";
    private const string TestKey4 = "KEY04";

    [NanoKey(obfuscationKey: TestKey1)]
    private interface I1 { }

    [NanoKey(obfuscationKey: TestKey2)]
    private interface I2 { }

    [NanoKey(obfuscationKey: TestKey3)]
    private abstract class BC : I1 { } // Overrides key from the interface

    [NanoKey(obfuscationKey: TestKey4)]
    private class C1 : BC { } // Overrides key from the base class

    private class C2 : BC { } // Key from the base class should be used.

    private class C3 : I1, I2 { } // This is not allowed

#pragma warning disable S2094 // Type used in a test
    private class C4 { } // No key
#pragma warning restore S2094


    public NanoExtensionsTests()
    {
        Nano.Initialize(_characterSets, _placeholderSets, NegativeSign);
    }

    [Theory]
    [InlineData(typeof(I1), TestKey1)]
    [InlineData(typeof(BC), TestKey3)]
    [InlineData(typeof(C1), TestKey4)]
    [InlineData(typeof(C2), TestKey3)] // No key was defined in the class, so it should use the key from the base class.
    [InlineData(typeof(C3), TestKey1)] // The type implement more than one interface, so it will use the key from the first one.
    public void Obfuscate_Deobfuscate_Should_Obfuscate_Number_With_Type(Type type, string keyToBeUsed)
    {
        // Arrange

        var number = 12345L;

        // Act

        var obfuscated = number.Obfuscate(type);
        var original = obfuscated.Deobfuscate(type);

        // Assert

        original.Should().Be(number);

        Nano.Obfuscate(number, keyToBeUsed).Should().Be(obfuscated);
        Nano.Deobfuscate(obfuscated, keyToBeUsed).Should().Be(number);
    }

    [Fact]
    public void Obfuscate_Deobfuscate_Should_Obfuscate_Number_With_Generics()
    {
        // Arrange

        var number = 12345L;

        // Act

        var obfuscated = number.Obfuscate<C2>();
        var original = obfuscated.Deobfuscate<C2>();

        // Assert

        original.Should().Be(number);

        Nano.Obfuscate(number, TestKey3).Should().Be(obfuscated);
        Nano.Deobfuscate(obfuscated, TestKey3).Should().Be(number);
    }

    [Fact]
    public void Obfuscate_Should_Throw_Exception_When_Type_Has_No_Key()
    {
        // Act

        Action act = () => 1L.Obfuscate<C4>();

        // Assert

        act.Should().Throw<InvalidOperationException>().WithMessage("No NanoKeyAttributes found for the type.");
    }
}

