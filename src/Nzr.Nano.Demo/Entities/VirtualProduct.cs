namespace Nzr.Nano.Demo.Entities;

/// <summary>
/// Represents a virtual product with an identifier.
/// Implements <see cref="IProduct"/>.
/// </summary>
public class VirtualProduct : IProduct
{
    /// <summary>
    /// The unique identifier for the virtual product.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The category of the product.
    /// </summary>
    public ProductCategory? Category { get; set; }
}
