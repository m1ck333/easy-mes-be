namespace AlGreenMES.Modules.Identity.Domain.Entities;

public class UserProcess
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ProcessId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public User User { get; private set; } = null!;

    private UserProcess()
    {
    }

    internal static UserProcess Create(Guid tenantId, Guid userId, Guid processId)
    {
        return new UserProcess
        {
            TenantId = tenantId,
            UserId = userId,
            ProcessId = processId
        };
    }
}
