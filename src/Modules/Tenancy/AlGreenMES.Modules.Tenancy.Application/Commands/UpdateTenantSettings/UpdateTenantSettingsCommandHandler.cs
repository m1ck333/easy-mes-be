using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.BuildingBlocks.Common.Interfaces;
using AlGreenMES.Modules.Tenancy.Application.DTOs;
using AlGreenMES.Modules.Tenancy.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Commands.UpdateTenantSettings;

public class UpdateTenantSettingsCommandHandler : IRequestHandler<UpdateTenantSettingsCommand, TenantSettingsDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantSettingsCommandHandler(ITenantRepository tenantRepository, IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantSettingsDto> Handle(UpdateTenantSettingsCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new DomainException("TENANT_NOT_FOUND", $"Tenant with id '{request.TenantId}' was not found.");

        if (tenant.Settings is null)
            throw new DomainException("TENANT_SETTINGS_NOT_FOUND", "Tenant settings were not found.");

        tenant.Settings.Update(
            request.DefaultWarningDays,
            request.DefaultCriticalDays,
            request.WarningColor,
            request.CriticalColor);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TenantSettingsDto(
            tenant.Settings.Id,
            tenant.Settings.TenantId,
            tenant.Settings.DefaultWarningDays,
            tenant.Settings.DefaultCriticalDays,
            tenant.Settings.WarningColor,
            tenant.Settings.CriticalColor);
    }
}
