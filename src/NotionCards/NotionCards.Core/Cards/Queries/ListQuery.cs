using NotionCards.Core.Entities;

namespace NotionCards.Core.Cards.Queries;

public enum OrderingType
{
  Ascending,
  Descending
}

public class ListQuery
{
  public OrderingType? Order { get; set; }
  public int? MaxCount { get; set; }
  public string? NextPageToken { get; set; }

  private const int DefaultMaxCount = 10;
  private const OrderingType DefaultOrderingType = OrderingType.Ascending;
  private const int DefaultLastId = 0;

  public void Deconstruct(out int maxCount, out int? lastId, out OrderingType order)
  {
    if (!string.IsNullOrWhiteSpace(NextPageToken))
    {
      var token = PaginationToken.FromString(NextPageToken);
      maxCount = token.MaxCount.GetValueOrDefault(DefaultMaxCount);
      lastId = token.LastId.GetValueOrDefault(DefaultLastId);
      order = token.Order.GetValueOrDefault(DefaultOrderingType);
      return;
    }

    maxCount = MaxCount.GetValueOrDefault(DefaultMaxCount);
    lastId = DefaultLastId;
    order = Order.GetValueOrDefault(DefaultOrderingType);
  }
}