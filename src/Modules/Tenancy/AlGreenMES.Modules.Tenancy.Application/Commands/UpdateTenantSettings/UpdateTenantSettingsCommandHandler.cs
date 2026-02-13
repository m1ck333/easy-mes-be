using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.BuildingBlocks.Common.Interfaces;
using AlGreenMES.Modules.Tenancy.Application.DTOs;
using AlGreenMES.Modules.Tenancy.Domain.Repositories;
using Mapster;
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
            ?? throw new NotFoundException("Tenant", request.TenantId);

        if (tenant.Settings is null)
            throw new NotFoundException("Tenant settings were not found.");

        tenant.Settings.Update(
            request.DefaultWarningDays,
            request.DefaultCriticalDays,
            request.WarningColor,
            request.CriticalColor);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tenant.Settings.Adapt<TenantSettingsDto>();
    }
}
