using Nzr.Nano.Extensions;

namespace Nzr.Nano.Demo.Entities;

/// <summary>
/// Represents a product with an identifier.
/// This is an interface for a product entity.
/// </summary>
[NanoKey("banana1")]
public interface IProduct
{
    /// <summary>
    /// The unique identifier for the product.
    /// </summary>
    long Id { get; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// The category of the product.
    /// </summary>
    public ProductCategory? Category { get; set; }
}
