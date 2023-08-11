namespace Nexus.PeopleAPI;

[Obsolete("Use Nexus.Common.Contracts.PaginatedList instead")]
public class PaginatedList<T>
{
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public List<T> Items { get; private set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }

    public bool HasPreviousPage => (PageIndex > 1);

    public bool HasNextPage => (PageIndex < TotalPages);

    public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
    {
        int count = source.Count();
        List<T> items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}