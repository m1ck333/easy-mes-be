namespace AlGreenMES.Modules.Production.Application.Interfaces;

public interface IReferenceCheckService
{
    Task<int> CountCategoryOrderReferencesAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<int> CountProcessOrderReferencesAsync(Guid processId, CancellationToken cancellationToken = default);
    Task NullifyCategoryReferencesAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task NullifyProcessReferencesAsync(Guid processId, CancellationToken cancellationToken = default);
}
