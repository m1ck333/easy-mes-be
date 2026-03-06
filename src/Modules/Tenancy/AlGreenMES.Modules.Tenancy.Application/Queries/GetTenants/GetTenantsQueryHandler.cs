using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Tenancy.Application.DTOs;
using AlGreenMES.Modules.Tenancy.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenants;

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, PagedResult<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantsQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<PagedResult<TenantDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var result = await _tenantRepository.GetPagedAsync(
            request.IsActive, request.Search,
            request.GetCreatedFromUtc(), request.GetCreatedToUtc(),
            request.GetPage(), request.GetPageSize(), cancellationToken);

        return result.MapItems(t => t.Adapt<TenantDto>());
    }
}
