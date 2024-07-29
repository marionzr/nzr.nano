# Nzr.Nano

Nzr.Nano is a lightweight and configurable library for obfuscating and deobfuscating numeric values. This project includes three key components:

1. **Nzr.Nano** - The core library for obfuscation and deobfuscation.
2. **Nzr.Nano.Generator** - A tool to generate valid character and placeholder sets (charsets).
3. **Nzr.Nano.Playground** - A web application to experiment with charset configurations and obfuscation/deobfuscation methods.

---

## Getting Started

### Installation
To install the core Nzr.Nano library, use the NuGet Package Manager:

```bash
Install-Package Nzr.Nano
```

To use the generator and playground, clone the repository and build the respective projects.

---

## Nzr.Nano.Generator

![Nzr.Nano.Generator](assets/generator.png)

### Overview
`Nzr.Nano.Generator` is a class library/console application designed to help developers create valid character and placeholder sets for use with the Nzr.Nano library. The generated charsets can be saved in a configuration file (e.g., `appsettings.json`) and used to initialize `Nano` with `NanoOptions`.

### How to Use

#### Step 1: Generate Charsets
You can generate charsets programmatically using the `NanoGenerator.Generate` method:

```csharp
using Nzr.Nano.Generator;

var settings = NanoGenerator.Generate();

// Output the generated settings as JSON
var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);
```

This will output a JSON object containing valid `CharacterSets` and `PlaceHolderCharSets`.

#### Step 2: Save Charsets to appsettings.json
Copy the generated JSON and add it to your `appsettings.json` under a section named `NanoOptions`:

```json
{
  "nanoOptions": {
    "characterSets": [
      "KP5Q7M431RXUYC9E8NWS2BT6J0",
      "41UN087DVKZQ6X3BGES9WTMPJ2",
      "3M0TD45PXESNQ7RFKUJVB92HCZ",
      "E6JX7G1RYBWFCPZ85KQN0H423D",
      "KX0UW3SC2EFDYT1QNZ9JRPB7V4",
      "PF4NJGX10M8U7SWEZVYC359THQ",
      "4CFJ1RBHM6KZT2WSG5Y30U8NQ7",
      "2BHFTQW98C30RX6JUGZEY4N57M",
      "U8QV14EDT25NBPWY36CFMJX9H7",
      "YNXSFH4J8RCWMPU37QVGKB21D5"
    ],
    "placeHolderCharSets": [
      "HGDVZF",
      "YRCH5F",
      "W61G8Y",
      "UMTV9S",
      "6GHM85",
      "K6B2RD",
      "DVEP9X",
      "KPVS1D",
      "GZRK0S",
      "E6T09Z"
    ],
    "negativeSign": "-"
  }
}
```

#### Step 3: Initialize Nano
Read the `NanoOptions` from the configuration file to initialize `Nano`:

```csharp
using Microsoft.Extensions.Configuration;
using Nzr.Nano;

// Build configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Bind NanoOptions from configuration
var nanoOptions = configuration.GetSection("NanoOptions").Get<NanoOptions>();

// Initialize Nano
Nano.Initialze(nanoOptions);
```

---

## Nzr.Nano.Playground

![Nzr.Nano.Playground](assets/playground.png)

### Overview
`Nzr.Nano.Playground` is a web-based application that allows developers to:

- Generate and modify charsets.
- Test obfuscation and deobfuscation methods interactively.

### How to Use

1. **Run the Playground**
   - Open the `Nzr.Nano.Playground` project in your IDE.
   - Build and run the project.
   - Access the application in your browser (e.g., `http://localhost:5000`).

2. **Generate Charsets**
   - Click the "Generate Charsets" button to create a new set of character and placeholder sets.
   - Copy the generated JSON from the "Charsets" textarea.

3. **Apply Generated Charsets**
   - Save the copied JSON to your `appsettings.json` file under the `NanoOptions` section.

4. **Test Obfuscation and Deobfuscation**
   - Use the obfuscation and deobfuscation sections to test your `NanoOptions` configuration.
   - Input a key and number to obfuscate, or an obfuscated value to deobfuscate.

---

## Examples

### Obfuscate a Number
```csharp
var obfuscated = nano.Obfuscate("myKey", 12345);
Console.WriteLine(obfuscated); // Output: Obfuscated value (depends on the charsets)
```

### Deobfuscate a Number
```csharp
var original = nano.Deobfuscate("myKey", obfuscated);
Console.WriteLine(original); // Output: 12345
```

---

## Contributing
Contributions are welcome! Feel free to submit issues or pull requests.

---

## License
Nzr.Nano is licensed under the Apache License, Version 2.0, January 2004. You may obtain a copy of the License at:

```
http://www.apache.org/licenses/LICENSE-2.0
```
