using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.BuildingBlocks.Common.Pagination;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize < 1 ? 20 : pageSize > 100 ? 100 : pageSize;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<T>.Create(items, totalCount, safePage, safePageSize);
    }

    public static IOrderedQueryable<T> ApplySort<T, TKey>(
        this IQueryable<T> query,
        string? sortBy,
        string targetField,
        bool isDescending,
        Expression<Func<T, TKey>> keySelector)
    {
        if (string.Equals(sortBy, targetField, StringComparison.OrdinalIgnoreCase))
            return isDescending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        return (IOrderedQueryable<T>)query;
    }
}
