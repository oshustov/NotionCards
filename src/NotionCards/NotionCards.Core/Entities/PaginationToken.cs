using System.Text;

namespace NotionCards.Core.Entities;

public class PaginationToken
{
  public int? MaxCount { get; set; }
  public int? LastGivenId { get; set; }

  public static PaginationToken Start(int maxCount) => new() {LastGivenId = null, MaxCount = maxCount};
  public static PaginationToken All() => new() {LastGivenId = null, MaxCount = null};
  public PaginationToken ContinueFrom(int lastGivenId) => new PaginationToken() { LastGivenId = lastGivenId, MaxCount = MaxCount };

  public static PaginationToken FromStringToken(string base64Token)
  {
    var bytes = Convert.FromBase64String(base64Token);
    var data = Encoding.UTF8.GetString(bytes);

    var parts = data.Split(':', StringSplitOptions.RemoveEmptyEntries);
    if (parts is not {Length: > 1})
      throw new ArgumentException("Unable to create a PaginationToken from the provided string");

    var token = new PaginationToken();
    if (int.TryParse(parts[0], out var maxCount))
      token.MaxCount = maxCount;
    else
      throw new ArgumentException("Invalid token part value for MaxCount");

    if (int.TryParse(parts[1], out var lastGivenId))
      token.LastGivenId = lastGivenId;
    else
      throw new ArgumentException("Invalid token part value for LastGivenId");

    return token;
  }

  public string GetString()
  {
    var data = $"{MaxCount}:{LastGivenId}";
    var bytes = Encoding.UTF8.GetBytes(data);

    return Convert.ToBase64String(bytes);
  }
}