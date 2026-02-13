using MediatR;

namespace AlGreenMES.BuildingBlocks.Common.Pagination;

public abstract record PagedQuery<TResponse> : IRequest<TResponse>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }

    public int GetPage() => Page < 1 ? 1 : Page;
    public int GetPageSize() => PageSize < 1 ? 20 : PageSize > 100 ? 100 : PageSize;
}
