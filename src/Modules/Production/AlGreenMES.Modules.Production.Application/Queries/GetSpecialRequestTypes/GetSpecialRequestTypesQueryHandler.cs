using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetSpecialRequestTypes;

public class GetSpecialRequestTypesQueryHandler : IRequestHandler<GetSpecialRequestTypesQuery, IReadOnlyList<SpecialRequestTypeDto>>
{
    private readonly ISpecialRequestTypeRepository _repository;

    public GetSpecialRequestTypesQueryHandler(ISpecialRequestTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SpecialRequestTypeDto>> Handle(GetSpecialRequestTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await _repository.GetByTenantIdAsync(request.TenantId, cancellationToken);

        return types.Select(srt => new SpecialRequestTypeDto(
            srt.Id, srt.TenantId, srt.Code, srt.Name, srt.Description,
            srt.AddsProcesses, srt.RemovesProcesses, srt.OnlyProcesses,
            srt.IgnoresDependencies, srt.IsActive
        )).ToList();
    }
}
