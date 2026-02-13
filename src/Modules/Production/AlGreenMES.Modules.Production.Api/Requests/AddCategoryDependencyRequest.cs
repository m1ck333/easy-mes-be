namespace AlGreenMES.Modules.Production.Api.Requests;

public record AddCategoryDependencyRequest(
    Guid ProcessId,
    Guid DependsOnProcessId);
