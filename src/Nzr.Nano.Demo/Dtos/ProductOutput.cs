using Nzr.Nano.Demo.Entities;
using Nzr.Nano.Extensions;

namespace Nzr.Nano.Demo.Dtos;

/// <summary>
/// API output for Product entity.
/// </summary>
public class ProductOutput
{
    /// <summary>
    /// The obfuscated unique identifier.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    public string? Name { get; init; }

    public ProductCategoryOutput? Category { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="ProductOutput"/>.
    /// </summary>
    /// <param name="product">The internal product entity</param>
    public ProductOutput(IProduct product)
    {
        Id = product.Id.Obfuscate<IProduct>();
        Name = product.Name;
        Category = product.Category != null ? new ProductCategoryOutput(product.Category) : null;
    }
}
