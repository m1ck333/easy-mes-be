using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletActiveWork;

public class GetTabletActiveWorkQueryHandler : IRequestHandler<GetTabletActiveWorkQuery, IReadOnlyList<ProcessGroupDto<TabletActiveWorkDto>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly ISpecialRequestTypeRepository _specialRequestTypeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProcessRepository _processRepository;

    public GetTabletActiveWorkQueryHandler(
        IOrderRepository orderRepository,
        IProductCategoryRepository categoryRepository,
        ISpecialRequestTypeRepository specialRequestTypeRepository,
        IUserRepository userRepository,
        IProcessRepository processRepository)
    {
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
        _specialRequestTypeRepository = specialRequestTypeRepository;
        _userRepository = userRepository;
        _processRepository = processRepository;
    }

    public async Task<IReadOnlyList<ProcessGroupDto<TabletActiveWorkDto>>> Handle(GetTabletActiveWorkQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithProcessesAsync(request.UserId, cancellationToken);
        if (user == null) return [];

        var userProcessIds = user.GetProcessIds();
        if (userProcessIds.Count == 0) return [];

        var orders = await _orderRepository.GetActiveOrdersWithProcessesAsync(request.TenantId, cancellationToken);

        var specialRequestTypes = await _specialRequestTypeRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);
        var srLookup = specialRequestTypes.ToDictionary(s => s.Id, s => s.Name);

        var result = new List<ProcessGroupDto<TabletActiveWorkDto>>();

        foreach (var processId in userProcessIds)
        {
            var prodProcess = await _processRepository.GetByIdAsync(processId, cancellationToken);
            if (prodProcess == null) continue;

            var items = new List<TabletActiveWorkDto>();

            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    var category = await _categoryRepository.GetByIdWithDetailsAsync(item.ProductCategoryId, cancellationToken);

                    var specialRequestNames = item.SpecialRequests
                        .Select(sr => srLookup.GetValueOrDefault(sr.SpecialRequestTypeId, ""))
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList();

                    var completedCount = item.Processes.Count(p =>
                        p.Status == ProcessStatus.Completed || p.Status == ProcessStatus.Withdrawn);
                    var totalCount = item.Processes.Count(p => !p.IsWithdrawn);

                    foreach (var process in item.Processes)
                    {
                        if (process.ProcessId != processId) continue;
                        if (process.Status != ProcessStatus.InProgress) continue;

                        var hasSubProcesses = process.SubProcesses.Any(sp => !sp.IsWithdrawn);

                        bool isTimerRunning;
                        DateTime? currentLogStartedAt;
                        int totalDuration;
                        List<TabletSubProcessDto> subDtos;

                        if (hasSubProcesses)
                        {
                            var activeSub = process.SubProcesses
                                .FirstOrDefault(sp => sp.Status == SubProcessStatus.InProgress);
                            var openLog = activeSub?.GetOpenLog();
                            isTimerRunning = openLog != null;
                            currentLogStartedAt = openLog?.StartTime;
                            totalDuration = process.SubProcesses.Sum(sp => sp.TotalDurationMinutes);
                            subDtos = process.SubProcesses.Select(sp =>
                            {
                                var spOpenLog = sp.GetOpenLog();
                                return new TabletSubProcessDto(
                                    sp.Id,
                                    sp.SubProcessId,
                                    sp.Status,
                                    sp.TotalDurationMinutes,
                                    sp.IsWithdrawn,
                                    sp.Status == SubProcessStatus.InProgress && spOpenLog != null,
                                    spOpenLog?.StartTime
                                );
                            }).ToList();
                        }
                        else
                        {
                            isTimerRunning = !process.PausedAt.HasValue;
                            totalDuration = process.TotalDurationMinutes;
                            currentLogStartedAt = isTimerRunning
                                ? (process.ResumedAt ?? process.StartedAt)
                                : null;
                            subDtos = new List<TabletSubProcessDto>();
                        }

                        var dto = process.Adapt<TabletActiveWorkDto>() with
                        {
                            ProductCategoryName = category?.Name,
                            SpecialRequestNames = specialRequestNames,
                            CompletedProcessCount = completedCount,
                            TotalProcessCount = totalCount,
                            TotalDurationMinutes = totalDuration,
                            IsTimerRunning = isTimerRunning,
                            CurrentLogStartedAt = currentLogStartedAt,
                            SubProcesses = subDtos
                        };
                        items.Add(dto);
                    }
                }
            }

            result.Add(new ProcessGroupDto<TabletActiveWorkDto>(
                prodProcess.Id, prodProcess.Code, prodProcess.Name, prodProcess.SequenceOrder, items));
        }

        return result.OrderBy(g => g.SequenceOrder).ToList();
    }
}
