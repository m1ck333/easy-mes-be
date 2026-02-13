using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetSpecialRequestTypes;

public record GetSpecialRequestTypesQuery(Guid TenantId) : IRequest<IReadOnlyList<SpecialRequestTypeDto>>;
