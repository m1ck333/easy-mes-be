using AlGreenMES.Modules.Tenancy.Application.DTOs;
using AlGreenMES.Modules.Tenancy.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenantByCode;

public class GetTenantByCodeQueryHandler : IRequestHandler<GetTenantByCodeQuery, TenantDto?>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByCodeQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantDto?> Handle(GetTenantByCodeQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (tenant is null)
            return null;

        return new TenantDto(
            tenant.Id,
            tenant.Name,
            tenant.Code,
            tenant.IsActive,
            tenant.CreatedAt,
            tenant.UpdatedAt);
    }
}
