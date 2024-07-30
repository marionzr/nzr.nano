using Nzr.Nano.Demo.Entities;
using Nzr.Nano.Extensions;

namespace Nzr.Nano.Demo.Dtos;

/// <summary>
/// API output for ProductCategory entity.
/// </summary>
public class ProductCategoryOutput
{
    /// <summary>
    /// The obfuscated unique identifier.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// The name of the category.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="ProductCategoryOutput"/>.
    /// </summary>
    /// <param name="product">The internal product entity</param>
    public ProductCategoryOutput(ProductCategory productCategory)
    {
        Id = productCategory.Id.Obfuscate<ProductCategory>();
        Name = productCategory.Name;
    }
}
