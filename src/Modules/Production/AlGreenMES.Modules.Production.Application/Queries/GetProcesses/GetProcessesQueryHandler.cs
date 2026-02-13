using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProcesses;

public class GetProcessesQueryHandler : IRequestHandler<GetProcessesQuery, PagedResult<ProcessDto>>
{
    private readonly IProcessRepository _processRepository;

    public GetProcessesQueryHandler(IProcessRepository processRepository)
    {
        _processRepository = processRepository;
    }

    public async Task<PagedResult<ProcessDto>> Handle(GetProcessesQuery request, CancellationToken cancellationToken)
    {
        var result = await _processRepository.GetPagedAsync(
            request.TenantId, request.IsActive, request.Search,
            request.GetPage(), request.GetPageSize(), cancellationToken);

        return result.MapItems(p => p.Adapt<ProcessDto>());
    }
}
