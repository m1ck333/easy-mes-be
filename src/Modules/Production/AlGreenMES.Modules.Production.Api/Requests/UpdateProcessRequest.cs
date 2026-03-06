namespace AlGreenMES.Modules.Production.Api.Requests;

public record UpdateProcessSubProcessAddInput(string Name, int SequenceOrder);

public record UpdateProcessRequest(
    string Name,
    int SequenceOrder,
    List<UpdateProcessSubProcessAddInput>? AddSubProcesses = null,
    List<Guid>? DeactivateSubProcessIds = null);
