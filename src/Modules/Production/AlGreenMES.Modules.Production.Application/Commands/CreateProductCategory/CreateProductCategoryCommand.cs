using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Enums;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.CreateProductCategory;

public record CreateProductCategoryCommand(
    Guid TenantId,
    string Name,
    string? Description,
    Guid? CreatedByUserId,
    List<ProcessInput>? Processes,
    List<DependencyInput>? Dependencies) : IRequest<ProductCategoryDetailDto>;

public record ProcessInput(Guid ProcessId, int SequenceOrder, ComplexityType? DefaultComplexity);
public record DependencyInput(Guid ProcessId, Guid DependsOnProcessId);
