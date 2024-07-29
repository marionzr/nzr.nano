namespace Nzr.Nano.Extensions;

/// <summary>
/// Specifies the obfuscation key to be used with the decorated type (interface or class)
/// for obfuscating or deobfuscating numbers using the <see cref="Nzr.Nano"/> library.
/// </summary>
/// <remarks>
/// This attribute is intended to simplify the usage of obfuscation keys by associating
/// a specific key with a type. An extension method can leverage this attribute to
/// retrieve the key and use it directly as the obfuscation key.
/// </remarks>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class NanoKeyAttribute : Attribute
{
    /// <summary>
    /// Gets the obfuscation key associated with the decorated type.
    /// </summary>
    public string ObfuscationKey { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NanoKeyAttribute"/> class with a specified obfuscation key.
    /// </summary>
    /// <param name="obfuscationKey">The obfuscation key to associate with the type.</param>
    public NanoKeyAttribute(string obfuscationKey)
    {
        ObfuscationKey = obfuscationKey ?? throw new ArgumentNullException(nameof(obfuscationKey), "Obfuscation key cannot be null.");
    }
}
