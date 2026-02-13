namespace AlGreenMES.BuildingBlocks.Common.Interfaces;

/// <summary>
/// Interface for entities that track audit information.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
    Guid? CreatedByUserId { get; }
    Guid? UpdatedByUserId { get; }
}
