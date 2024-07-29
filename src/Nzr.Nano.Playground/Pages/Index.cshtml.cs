using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nzr.Nano.Generator;

namespace Nzr.Nano.Playground.Pages;

/// <summary>
/// IndexModel is the Razor Page Model for the Nano Playground page.
/// It handles HTTP requests for various actions such as generating nano settings,
/// applying nano options, obfuscating, and deobfuscating values.
/// </summary>
[SuppressMessage("Minor Code Smell", "S2325")]
public class IndexModel : PageModel
{
    /// <summary>
    /// Handles the GET request to the Index page.
    /// </summary>
    /// <returns>An IActionResult representing the page view.</returns>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// Generates new nano settings using the NanoGenerator and returns the result as JSON.
    /// Initializes the Nano options based on the generated settings.
    /// </summary>
    /// <returns>A JsonResult containing the generated settings in JSON format.</returns>
    public JsonResult OnGetGenerate()
    {
        var settings = NanoGenerator.Generate();
        Nano.Initialize(settings.NanoOptions);
        return new JsonResult(settings);
    }

    /// <summary>
    /// Applies nano options by deserializing the provided JSON string into NanoGenerator.Settings,
    /// then initializes Nano with those settings.
    /// </summary>
    /// <param name="nanoOptionsJson">The JSON string representing the nano options.</param>
    /// <returns>A JsonResult containing the deserialized settings in JSON format.</returns>
    public JsonResult OnPostNanoOptions(string nanoOptionsJson)
    {
        var settings = JsonSerializer.Deserialize<NanoGenerator.Settings>(nanoOptionsJson)!;
        Nano.Initialize(settings.NanoOptions);
        return new JsonResult(settings);
    }

    /// <summary>
    /// Obfuscates a number based on the provided key, number, and minimum length.
    /// </summary>
    /// <param name="key">The key used for obfuscation.</param>
    /// <param name="number">The number to obfuscate.</param>
    /// <param name="minLength">The minimum length for the obfuscated value.</param>
    /// <returns>A JsonResult containing the obfuscated value.</returns>
    public JsonResult OnPostObfuscate(string key, long number, int minLength)
    {
        try
        {
            var obfuscated = Nano.Obfuscate(number, key, minLength);
            return new JsonResult(new { obfuscated });
        }
        catch (Exception e)
        {
            return new JsonResult(new { error = e });
        }
    }

    /// <summary>
    /// Deobfuscates the provided obfuscated value using the provided key.
    /// </summary>
    /// <param name="key">The key used for deobfuscation.</param>
    /// <param name="obfuscatedValue">The obfuscated value to deobfuscate.</param>
    /// <returns>A JsonResult containing the original number or an error message.</returns>
    public JsonResult OnPostDeobfuscate(string key, string obfuscatedValue)
    {
        try
        {
            var originalNumber = Nano.Deobfuscate(obfuscatedValue, key);
            return new JsonResult(new { originalNumber });
        }
        catch (Exception e)
        {
            return new JsonResult(new { error = e });
        }
    }
}
