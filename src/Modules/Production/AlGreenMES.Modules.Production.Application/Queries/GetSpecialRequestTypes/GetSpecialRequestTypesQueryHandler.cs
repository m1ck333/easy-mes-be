using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetSpecialRequestTypes;

public class GetSpecialRequestTypesQueryHandler : IRequestHandler<GetSpecialRequestTypesQuery, PagedResult<SpecialRequestTypeDto>>
{
    private readonly ISpecialRequestTypeRepository _repository;

    public GetSpecialRequestTypesQueryHandler(ISpecialRequestTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SpecialRequestTypeDto>> Handle(GetSpecialRequestTypesQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(
            request.TenantId, request.IsActive, request.Search,
            request.GetCreatedFromUtc(), request.GetCreatedToUtc(),
            request.SortBy, request.IsDescending,
            request.GetPage(), request.GetPageSize(), cancellationToken);

        return result.MapItems(srt => srt.Adapt<SpecialRequestTypeDto>());
    }
}
