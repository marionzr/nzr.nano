namespace Nzr.Nano.Demo.Entities;

/// <summary>
/// Represents a product entity.
/// Inherits from <see cref="BaseEntity"/> and implements <see cref="IProduct"/>.
/// </summary>
public class Product : BaseEntity, IProduct
{
    /// <summary>
    /// The name of the product.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The category of the product.
    /// </summary>
    public ProductCategory? Category { get; set; }
}
