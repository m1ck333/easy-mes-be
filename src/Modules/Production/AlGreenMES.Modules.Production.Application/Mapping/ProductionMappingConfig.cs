using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Entities;
using Mapster;

namespace AlGreenMES.Modules.Production.Application.Mapping;

public static class ProductionMappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Process, ProcessDto>()
            .Map(dest => dest.SubProcesses, src => src.SubProcesses);

        config.NewConfig<SubProcess, SubProcessDto>();

        config.NewConfig<ProductCategory, ProductCategoryDto>();

        config.NewConfig<ProductCategory, ProductCategoryDetailDto>()
            .Map(dest => dest.Processes, src => src.Processes)
            .Map(dest => dest.Dependencies, src => src.Dependencies);

        config.NewConfig<ProductCategoryProcess, ProductCategoryProcessDto>()
            .Map(dest => dest.ProcessCode, src => src.Process != null ? src.Process.Code : null)
            .Map(dest => dest.ProcessName, src => src.Process != null ? src.Process.Name : null);

        config.NewConfig<ProductCategoryDependency, ProductCategoryDependencyDto>()
            .Map(dest => dest.ProcessCode, src => src.Process != null ? src.Process.Code : null)
            .Map(dest => dest.DependsOnProcessCode, src => src.DependsOnProcess != null ? src.DependsOnProcess.Code : null);

        config.NewConfig<SpecialRequestType, SpecialRequestTypeDto>();
    }
}
