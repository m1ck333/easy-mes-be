using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class OrderItemSubProcess : TenantEntity
{
    public Guid OrderItemProcessId { get; private set; }
    public Guid SubProcessId { get; private set; }
    public SubProcessStatus Status { get; private set; }
    public int TotalDurationMinutes { get; private set; }
    public bool IsWithdrawn { get; private set; }
    public DateTime? WithdrawnAt { get; private set; }
    public Guid? WithdrawnByUserId { get; private set; }
    public string? WithdrawnReason { get; private set; }
    public Guid? StoppedByUserId { get; private set; }
    public string? StoppedReason { get; private set; }

    public OrderItemProcess OrderItemProcess { get; private set; } = null!;

    private readonly List<OrderItemSubProcessLog> _logs = new();
    public IReadOnlyCollection<OrderItemSubProcessLog> Logs => _logs.AsReadOnly();

    private OrderItemSubProcess()
    {
    }

    internal static OrderItemSubProcess Create(Guid tenantId, Guid orderItemProcessId, Guid subProcessId)
    {
        return new OrderItemSubProcess
        {
            TenantId = tenantId,
            OrderItemProcessId = orderItemProcessId,
            SubProcessId = subProcessId,
            Status = SubProcessStatus.Pending
        };
    }

    public void Start()
    {
        if (Status != SubProcessStatus.Pending)
            throw new DomainException("INVALID_STATUS", "Can only start pending sub-processes.");
        Status = SubProcessStatus.InProgress;
    }

    public void Complete()
    {
        Status = SubProcessStatus.Completed;
    }

    public void Stop(Guid userId, string reason)
    {
        Status = SubProcessStatus.Stopped;
        StoppedByUserId = userId;
        StoppedReason = reason;
    }

    public void Withdraw(Guid userId, string reason)
    {
        IsWithdrawn = true;
        WithdrawnAt = DateTime.UtcNow;
        WithdrawnByUserId = userId;
        WithdrawnReason = reason;
        Status = SubProcessStatus.Withdrawn;
    }

    public void AddDuration(int minutes)
    {
        TotalDurationMinutes += minutes;
    }

    public OrderItemSubProcessLog StartLog(Guid userId)
    {
        var openLog = _logs.FirstOrDefault(l => l.EndTime == null);
        if (openLog != null)
            throw new DomainException("LOG_ALREADY_OPEN", "There is already an open log entry for this sub-process.");

        var log = OrderItemSubProcessLog.Start(TenantId, Id, userId);
        _logs.Add(log);
        return log;
    }

    public OrderItemSubProcessLog? GetOpenLog()
    {
        return _logs.FirstOrDefault(l => l.EndTime == null);
    }
}
