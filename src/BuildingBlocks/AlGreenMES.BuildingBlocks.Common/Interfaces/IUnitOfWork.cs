namespace AlGreenMES.BuildingBlocks.Common.Interfaces;

/// <summary>
/// Unit of work pattern interface. Each module's DbContext implements this.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
