using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Domain.Repositories;
using AlGreenMES.Modules.Production.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Production.Infrastructure.Repositories;

public class ProcessRepository : IProcessRepository
{
    private readonly ProductionDbContext _dbContext;

    public ProcessRepository(ProductionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Process?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Processes
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Process?> GetByIdWithSubProcessesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Processes
            .Include(p => p.SubProcesses)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Process>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Processes
            .Include(p => p.SubProcesses)
            .Where(p => p.TenantId == tenantId)
            .OrderBy(p => p.SequenceOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Process process, CancellationToken cancellationToken = default)
    {
        await _dbContext.Processes.AddAsync(process, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _dbContext.Processes
            .AnyAsync(p => p.Code == normalizedCode && p.TenantId == tenantId, cancellationToken);
    }
}
