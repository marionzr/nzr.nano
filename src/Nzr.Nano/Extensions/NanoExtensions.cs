using System.Collections.Concurrent;
using System.Reflection;

namespace Nzr.Nano.Extensions;

/// <summary>
/// Provides extension methods to simplify the use of the Nano library for obfuscation and deobfuscation.
/// </summary>
public static class NanoExtensions
{
    // ConcurrentDictionary to cache the Nano keys by type
    private static readonly ConcurrentDictionary<Type, string> NanoKeyCache = new();

    /// <summary>
    /// Obfuscates a number using a key associated with the specified type.
    /// </summary>
    /// <typeparam name="T">The type whose Nano key will be used for obfuscation.</typeparam>
    /// <param name="number">The number to obfuscate.</param>
    /// <param name="minLength">The minimum length of the obfuscated string (default is 6).</param>
    /// <returns>The obfuscated string.</returns>
    public static string Obfuscate<T>(this long number, int minLength = 6)
    {
        return number.Obfuscate(typeof(T), minLength);
    }

    /// <summary>
    /// Obfuscates a number using a key associated with the specified type.
    /// </summary>
    /// <typeparam name="T">The type whose Nano key will be used for obfuscation.</typeparam>
    /// <param name="number">The number to obfuscate.</param>
    /// <param name="minLength">The minimum length of the obfuscated string (default is 6).</param>
    /// <returns>The obfuscated string.</returns>
    public static string Obfuscate<T>(this int number, int minLength = 6)
    {
        return number.Obfuscate(typeof(T), minLength);
    }

    /// <summary>
    /// Obfuscates a nullable number using a key associated with the specified type.
    /// </summary>
    /// <typeparam name="T">The type whose Nano key will be used for obfuscation.</typeparam>
    /// <param name="number">The nullable number to obfuscate.</param>
    /// <param name="minLength">The minimum length of the obfuscated string (default is 6).</param>
    /// <returns>The obfuscated string, or <c>null</c> if the input number is <c>null</c>.</returns>
    public static string? Obfuscate<T>(this long? number, int minLength = 6)
    {
        return number?.Obfuscate(typeof(T), minLength);
    }

    /// <summary>
    /// Obfuscates a nullable number using a key associated with the specified type.
    /// </summary>
    /// <typeparam name="T">The type whose Nano key will be used for obfuscation.</typeparam>
    /// <param name="number">The nullable number to obfuscate.</param>
    /// <param name="minLength">The minimum length of the obfuscated string (default is 6).</param>
    /// <returns>The obfuscated string, or <c>null</c> if the input number is <c>null</c>.</returns>
    public static string? Obfuscate<T>(this int? number, int minLength = 6)
    {
        return number?.Obfuscate(typeof(T), minLength);
    }

    /// <summary>
    /// Obfuscates a number using a key associated with the specified type.
    /// </summary>
    /// <param name="number">The number to obfuscate.</param>
    /// <param name="type">The type whose Nano key will be used for obfuscation.</param>
    /// <param name="minLength">The minimum length of the obfuscated string (default is 6).</param>
    /// <returns>The obfuscated string.</returns>
    public static string Obfuscate(this long number, Type type, int minLength = 6)
    {
        var obfuscationKey = GetNanoKey(type);
        var oNumber = Nano.Obfuscate(number, obfuscationKey, minLength);

        return oNumber;
    }

    /// <summary>
    /// Obfuscates a number using a key associated with the specified type.
    /// </summary>
    /// <param name="number">The number to obfuscate.</param>
    /// <param name="type">The type whose Nano key will be used for obfuscation.</param>
    /// <param name="minLength">The minimum length of the obfuscated string (default is 6).</param>
    /// <returns>The obfuscated string.</returns>
    public static string Obfuscate(this int number, Type type, int minLength = 6)
    {
        var obfuscationKey = GetNanoKey(type);
        var oNumber = Nano.Obfuscate(number, obfuscationKey, minLength);

        return oNumber;
    }

    /// <summary>
    /// Obfuscates a nullable number using a key associated with the specified type.
    /// </summary>
    /// <param name="number">The nullable number to obfuscate.</param>
    /// <param name="type">The type whose Nano key will be used for obfuscation.</param>
    /// <param name="minLength">The minimum length of the obfuscated string (default is 6).</param>
    /// <returns>The obfuscated string, or <c>null</c> if the input number is <c>null</c>.</returns>
    public static string? Obfuscate(this long? number, Type type, int minLength = 6)
    {
        if (number == null)
        {
            return null;
        }

        var obfuscationKey = GetNanoKey(type);
        var oNumber = Nano.Obfuscate(number.Value, obfuscationKey, minLength);

        return oNumber;
    }

    /// <summary>
    /// Deobfuscates an obfuscated string to retrieve the original number using a key associated with the specified type.
    /// </summary>
    /// <typeparam name="T">The type whose Nano key will be used for deobfuscation.</typeparam>
    /// <param name="obfuscatedString">The obfuscated string to deobfuscate.</param>
    /// <returns>The original number, or <c>null</c> if the input string is <c>null</c>.</returns>
    public static long? Deobfuscate<T>(this string? obfuscatedString)
    {
        return obfuscatedString?.Deobfuscate(typeof(T));
    }

    /// <summary>
    /// Deobfuscates an obfuscated string to retrieve the original number using a key associated with the specified type.
    /// </summary>
    /// <param name="obfuscatedString">The obfuscated string to deobfuscate.</param>
    /// <param name="type">The type whose Nano key will be used for deobfuscation.</param>
    /// <returns>The original number, or <c>null</c> if the input string is <c>null</c>.</returns>
    public static long? Deobfuscate(this string? obfuscatedString, Type type)
    {
        if (obfuscatedString == null)
        {
            return null;
        }

        var obfuscationKey = GetNanoKey(type);
        var originalNumber = Nano.Deobfuscate(obfuscatedString, obfuscationKey);

        return originalNumber;
    }

    /// <summary>
    /// Retrieves the Nano key for a given type by checking for the presence of <see cref="NanoKeyAttribute"/> on the type and its interfaces.
    /// </summary>
    /// <param name="type">The type to retrieve the Nano key from.</param>
    /// <returns>The Nano key as a string.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no NanoKeyAttribute is found or if multiple attributes are found at the same level of the hierarchy.
    /// </exception>
    internal static string GetNanoKey(Type type)
    {
        if (NanoKeyCache.TryGetValue(type, out var cachedKey))
        {
            return cachedKey;
        }

        // Start by getting all the NanoKeyAttributes from the type itself and its interfaces
        var attributes = new List<(Type Type, NanoKeyAttribute Attribute)>();

        var singleAttribute = type.GetCustomAttributes<NanoKeyAttribute>(false).SingleOrDefault();

        if (singleAttribute != null)
        {
            attributes.Add((type, singleAttribute));
        }
        else
        {
            // Include attributes from the current type
            foreach (var attribute in type.GetCustomAttributes<NanoKeyAttribute>(true))
            {
                attributes.Add((type, attribute));
            }

            // Include attributes from the interfaces of the current type
            foreach (var interfaceType in type.GetInterfaces())
            {
                foreach (var attribute in interfaceType.GetCustomAttributes<NanoKeyAttribute>(true))
                {
                    attributes.Add((interfaceType, attribute));
                }
            }
        }

        // If no attributes found, throw an exception
        if (attributes.Count == 0)
        {
            throw new InvalidOperationException("No NanoKeyAttributes found for the type.");
        }

        // Sort the attributes based on the hierarchy of the type (closest first)
        var sortedAttributes = attributes
            .OrderBy(a => GetTypeHierarchyDistance(a.Type, type))
            .ToList();

        // Return the obfuscation key of the closest attribute
        var obfuscationKey = sortedAttributes[0].Attribute.ObfuscationKey;

        // Cache the key for future use
        NanoKeyCache[type] = obfuscationKey;

        return obfuscationKey;
    }


    /// <summary>
    /// Calculates the hierarchy distance between two types for sorting purposes.
    /// </summary>
    /// <param name="fromType">The starting type in the hierarchy.</param>
    /// <param name="toType">The target type in the hierarchy.</param>
    /// <returns>The distance as an integer.</returns>
    private static int GetTypeHierarchyDistance(Type? fromType, Type toType)
    {
        // If both are the same, return 0 (this means 'fromType' is the same as 'toType')
        if (fromType == toType)
        {
            return 0;
        }

        // Calculate distance for interfaces and base types
        var distance = 0;

        // Traverse the inheritance hierarchy
        while (fromType != null)
        {
            if (fromType == toType)
            {
                return distance;
            }

            distance++;

            fromType = fromType.BaseType;
        }

        // If the type is not found in the hierarchy, it must be an interface, so check interfaces
        distance = 0;

        foreach (var iface in toType.GetInterfaces())
        {
            if (iface == fromType)
            {
                return distance;
            }

            distance++;
        }

        return distance;
    }
}
