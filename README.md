# Nzr.Nano

Nzr.Nano is a lightweight and configurable library designed for obfuscating and deobfuscating numeric values. It supports custom character and placeholder sets configurations, and negative sign handling, making it highly flexible for various use cases.

## Features

- Obfuscation and deobfuscation of numeric values.
- Configurable character sets for obfuscation.
- Placeholder handling for shorter obfuscated values.
- Support for negative numbers.

## Installation

Install the NuGet package:

```bash
Install-Package Nzr.Nano
```

## Getting Started

### Initialization

Nzr.Nano is a static class and requires initialization before use. You must provide character sets, placeholder sets, and a negative sign for obfuscation. Example:

```csharp
using Nzr.Nano;

var characterSets = new[]
{
    "H9MQB682DXPG0YKUTFS17CN54J",
    "TXMRD72S31Z6UEQ5K9B8F4NVHW",
    // Add additional character sets as needed
};

var placeholderSets = new[]
{
    "V3WERZ",
    "YPJ0GC",
    // Add additional placeholder sets as needed
};

var negativeSign = "-";

// Initialize the Nano library
Nano.Initialize(characterSets, placeholderSets, negativeSign);
```

### Obfuscating Numbers

Once initialized, you can obfuscate numeric values using the `Obfuscate` method. You can optionally specify a key and a minimum length for the obfuscated value. Note: The `minLength` must be at least the length of the placeholder set. For example, if the placeholder set is "V3WERZ", `minLength` must be at least 6.

```csharp
var number = 12345L;
var key = "MySecretKey";
var minLength = 6;

var obfuscated = Nano.Obfuscate(number, key, minLength);
Console.WriteLine(obfuscated); // Example output: "YPJ3WE"
```

### Deobfuscating Numbers

To retrieve the original value, use the `Deobfuscate` method with the obfuscated value and the same key used during obfuscation:

```csharp
var deobfuscated = Nano.Deobfuscate(obfuscated, key);
Console.WriteLine(deobfuscated); // Output: 12345
```

### Handling Negative Numbers

Negative numbers are supported and will be prefixed with the configured negative sign during obfuscation:

```csharp
var negativeNumber = -12345L;
var obfuscatedNegative = Nano.Obfuscate(negativeNumber, key);
var deobfuscatedNegative = Nano.Deobfuscate(obfuscatedNegative, key);

Console.WriteLine(obfuscatedNegative); // Example: "-YPJ3WE"
Console.WriteLine(deobfuscatedNegative); // Output: -12345
```

## Configuration Validation

Nzr.Nano validates configurations during initialization to ensure integrity. The following scenarios will throw exceptions:

1. Overlapping characters between character sets and placeholder sets.
2. Invalid negative sign (must be a single character and not present in any character set).
3. `minLength` smaller than the length of the placeholder set.

Example of validation failure:

```csharp
var invalidCharacterSets = new[] { "H9M0B", "0KM7F" }; // Overlap in '0'
var invalidNegativeSign = "--"; // Invalid negative sign

 // Throws ArgumentException
Nano.Initialize(invalidCharacterSets, placeholderSets, invalidNegativeSign);
```

### Customizing Minimum Length

```csharp
var shortNumber = 42L;
var minLength = 8;

string obfuscated = Nano.Obfuscate(shortNumber, key, minLength);
Console.WriteLine(obfuscated); // Obfuscated value with at least 8 characters
```

## Exception Handling

Nzr.Nano throws detailed exceptions to help identify issues during obfuscation, deobfuscation, or configuration:

- **ArgumentException**: Thrown for invalid configurations or obfuscation/deobfuscation errors.

Example:

```csharp
try
{
    Nano.Deobfuscate("Invalid#Value", key);
}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);
}
```

## License

Nzr.Nano is licensed under the [Apache License 2.0](LICENSE).
