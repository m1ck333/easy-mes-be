using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Production.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class OrderItemProcess : TenantEntity
{
    public Guid OrderItemId { get; private set; }
    public Guid ProcessId { get; private set; }
    public ComplexityType? Complexity { get; private set; }
    public bool ComplexityOverridden { get; private set; }
    public ProcessStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int TotalDurationMinutes { get; private set; }

    public bool IsWithdrawn { get; private set; }
    public DateTime? WithdrawnAt { get; private set; }
    public Guid? WithdrawnByUserId { get; private set; }
    public string? WithdrawnReason { get; private set; }

    public DateTime? BlockedAt { get; private set; }
    public Guid? BlockedByUserId { get; private set; }
    public string? BlockReason { get; private set; }
    public DateTime? UnblockedAt { get; private set; }
    public Guid? UnblockedByUserId { get; private set; }

    public DateTime? StoppedAt { get; private set; }
    public Guid? StoppedByUserId { get; private set; }
    public string? StoppedReason { get; private set; }

    public DateTime? PausedAt { get; private set; }
    public DateTime? ResumedAt { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public OrderItem OrderItem { get; private set; } = null!;

    private readonly List<OrderItemSubProcess> _subProcesses = new();
    public IReadOnlyCollection<OrderItemSubProcess> SubProcesses => _subProcesses.AsReadOnly();

    private OrderItemProcess()
    {
    }

    internal static OrderItemProcess Create(Guid tenantId, Guid orderItemId, Guid processId,
        ComplexityType? complexity, bool overridden)
    {
        return new OrderItemProcess
        {
            TenantId = tenantId,
            OrderItemId = orderItemId,
            ProcessId = processId,
            Complexity = complexity,
            ComplexityOverridden = overridden,
            Status = ProcessStatus.Pending
        };
    }

    public void Start()
    {
        if (Status != ProcessStatus.Pending)
            throw new DomainException("INVALID_STATUS", "Can only start pending processes.");
        Status = ProcessStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (_subProcesses.Any() && !_subProcesses.All(sp => sp.Status == SubProcessStatus.Completed || sp.Status == SubProcessStatus.Withdrawn))
            throw new DomainException("SUBPROCESSES_NOT_COMPLETE", "All sub-processes must be completed first.");
        Status = ProcessStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Block(Guid userId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("REASON_REQUIRED", "Block reason is required.");
        Status = ProcessStatus.Blocked;
        BlockedAt = DateTime.UtcNow;
        BlockedByUserId = userId;
        BlockReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unblock(Guid userId)
    {
        if (Status != ProcessStatus.Blocked)
            throw new DomainException("NOT_BLOCKED", "Process is not blocked.");
        Status = StartedAt.HasValue ? ProcessStatus.InProgress : ProcessStatus.Pending;
        UnblockedAt = DateTime.UtcNow;
        UnblockedByUserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Stop(Guid userId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("REASON_REQUIRED", "Stop reason is required.");
        Status = ProcessStatus.Stopped;
        StoppedAt = DateTime.UtcNow;
        StoppedByUserId = userId;
        StoppedReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw(Guid userId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("REASON_REQUIRED", "Withdrawal reason is required.");
        IsWithdrawn = true;
        WithdrawnAt = DateTime.UtcNow;
        WithdrawnByUserId = userId;
        WithdrawnReason = reason;
        Status = ProcessStatus.Withdrawn;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Pause()
    {
        if (PausedAt.HasValue)
            throw new DomainException("ALREADY_PAUSED", "Process is already paused.");

        // Accumulate current session duration in seconds
        var sessionStart = ResumedAt ?? StartedAt ?? DateTime.UtcNow;
        var sessionSeconds = (int)(DateTime.UtcNow - sessionStart).TotalSeconds;
        TotalDurationMinutes += sessionSeconds;

        PausedAt = DateTime.UtcNow;
        ResumedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResumeTimer()
    {
        if (!PausedAt.HasValue)
            throw new DomainException("NOT_PAUSED", "Process is not paused.");

        PausedAt = null;
        ResumedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddDuration(int minutes)
    {
        TotalDurationMinutes += minutes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void OverrideComplexity(ComplexityType complexity)
    {
        Complexity = complexity;
        ComplexityOverridden = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public OrderItemSubProcess AddSubProcess(Guid subProcessId)
    {
        if (_subProcesses.Any(sp => sp.SubProcessId == subProcessId))
            throw new DomainException("DUPLICATE_SUBPROCESS", "Sub-process already added.");

        var subProcess = OrderItemSubProcess.Create(TenantId, Id, subProcessId);
        _subProcesses.Add(subProcess);
        return subProcess;
    }
}
