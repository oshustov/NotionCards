using System.Text;
using System.Text.Json;
using NotionCards.Core.Cards.Queries;

namespace NotionCards.Core.Entities;

public class PaginationToken
{
  public int? MaxCount { get; set; }
  public int? LastId { get; set; }
  public OrderingType? Order { get; set; }

  public static PaginationToken Start(int maxCount) => new() {LastId = null, MaxCount = maxCount};
  public static PaginationToken All() => new() {LastId = null, MaxCount = null};
  public PaginationToken ContinueFrom(int lastGivenId) => new PaginationToken() { LastId = lastGivenId, MaxCount = MaxCount };

  public static PaginationToken FromString(string token)
  {
    var bytes = Convert.FromBase64String(token);
    var data = Encoding.UTF8.GetString(bytes);

    if (string.IsNullOrWhiteSpace(data))
      throw new ArgumentException("The provided value can't be used to create a PaginationToken instance");

    return JsonSerializer.Deserialize<PaginationToken>(data)!;
  }

  public string GetString()
  {
    var data = JsonSerializer.Serialize(this);
    var bytes = Encoding.UTF8.GetBytes(data);

    return Convert.ToBase64String(bytes);
  }
}