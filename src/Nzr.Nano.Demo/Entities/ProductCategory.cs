using Nzr.Nano.Extensions;

namespace Nzr.Nano.Demo.Entities;

/// <summary>
/// The category of the product
/// </summary>
[NanoKey("apple1")]
public class ProductCategory : BaseEntity
{
    /// <summary>
    /// The name of the category.
    /// </summary>
    public string? Name { get; set; }
}
