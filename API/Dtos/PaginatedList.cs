namespace API.Dtos;

public class PaginatedList<T>(List<T> items, int totalPages, int pageIndex, int pageSize) {
  public List<T> Items { get; set; } = items;
  public int PageIndex { get; set; } = pageIndex;
  public int PageSize { get; set; } = pageSize;
  public int TotalPages { get; set; } = totalPages;
  public bool HasPrev { get; set; }
  public bool HasNext { get; set; }

  public static PaginatedList<T> Create(IQueryable<T>? query, int pageIndex, int pageSize) {
    if (query != null) {
      var totalPages = (int)Math.Ceiling(query.Count() / (double)pageSize);
      var items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

      return new PaginatedList<T>(items, totalPages, pageIndex, pageSize);
    }
    return new PaginatedList<T>([], 0, 0, 0);
  }
}