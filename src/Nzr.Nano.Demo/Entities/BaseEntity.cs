namespace Nzr.Nano.Demo.Entities;

/// <summary>
/// The base class for entities, providing a common identifier.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset LastUpdatedAt { get; set; }
}
