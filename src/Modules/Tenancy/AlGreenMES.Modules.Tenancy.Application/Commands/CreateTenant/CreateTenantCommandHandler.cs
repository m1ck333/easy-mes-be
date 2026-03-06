using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.BuildingBlocks.Common.Interfaces;
using AlGreenMES.Modules.Tenancy.Application.DTOs;
using AlGreenMES.Modules.Tenancy.Domain.Entities;
using AlGreenMES.Modules.Tenancy.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantCommandHandler(ITenantRepository tenantRepository, IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var codeExists = await _tenantRepository.ExistsAsync(request.Code, cancellationToken);
        if (codeExists)
            throw new DomainException("TENANT_CODE_EXISTS", $"A tenant with code '{request.Code}' already exists.");

        var tenant = Tenant.Create(request.Name, request.Code);

        if (request.DefaultWarningDays.HasValue && tenant.Settings is not null)
        {
            tenant.Settings.Update(
                request.DefaultWarningDays.Value,
                request.DefaultCriticalDays ?? tenant.Settings.DefaultCriticalDays,
                request.WarningColor ?? tenant.Settings.WarningColor,
                request.CriticalColor ?? tenant.Settings.CriticalColor);
        }

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tenant.Adapt<TenantDto>();
    }
}
