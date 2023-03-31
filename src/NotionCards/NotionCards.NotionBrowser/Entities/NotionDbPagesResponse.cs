using System.Text.Json.Serialization;

namespace NotionCards.NotionBrowser.Entities;

public class NotionDbPagesResponse
{
  [JsonPropertyName("object")]
  public string Type { get; set; }

  [JsonPropertyName("results")]
  public IEnumerable<Result> Results { get; set; }
}

public class Result
{
  [JsonPropertyName("object")]
  public string Type { get; set; }

  [JsonPropertyName("id")]
  public string Id { get; set; }

  [JsonPropertyName("created_time")]
  public DateTime CreatedTime { get; set; }

  [JsonPropertyName("url")]
  public string Url { get; set; }
}