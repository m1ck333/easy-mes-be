using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;

namespace AlGreenMES.Modules.Orders.Application.Interfaces;

public interface IDashboardQueryService
{
    Task<IReadOnlyList<DeadlineWarningDto>> GetDeadlineWarningsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LiveViewProcessDto>> GetLiveViewAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkerStatusDto>> GetWorkersStatusAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PendingBlockRequestDto>> GetPendingBlockRequestsAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<DashboardStatisticsDto> GetDashboardStatisticsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
