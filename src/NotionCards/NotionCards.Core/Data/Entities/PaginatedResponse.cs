using NotionCards.Core.Entities;

namespace NotionCards.Storage.Entities;

public class PaginatedResponse<T>
{
  public PaginationToken? NextToken { get; set; }
  public T[] Result { get; set; } = Array.Empty<T>();
}