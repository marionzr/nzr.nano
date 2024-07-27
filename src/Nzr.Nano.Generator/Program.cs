using System.Text.Json;

namespace Nzr.Nano.Generator;

internal static class Program
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Entry point of the application.
    /// </summary>
#pragma warning disable S2190
    private static void Main()
#pragma warning restore S2190
    {
        // Infinite loop with a way to break
        while (true)
        {
            Console.WriteLine("=================================");
            var settings = NanoGenerator.Generate();
            var json = JsonSerializer.Serialize(settings, _jsonSerializerOptions);

            Console.WriteLine();
            Console.WriteLine(json);
            Console.WriteLine();
            Console.WriteLine("Type any key to generate a new character set, or press Ctrl+C to exit.");
            Console.ReadKey();
        }
    }
}
