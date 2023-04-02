using System.Text.Json.Serialization;

namespace NotionCards.NotionBrowser.Entities;

public class NotionResponse
{
  [JsonPropertyName("object")]
  public string Object { get; set; }

  [JsonPropertyName("results")]
  public IEnumerable<Result> Results { get; set; }
}

public partial class Result
{
  [JsonPropertyName("object")]
  public string Object { get; set; }

  [JsonPropertyName("type")]
  public string Type { get; set; }

  [JsonPropertyName("id")]
  public string Id { get; set; }

  [JsonPropertyName("created_time")]
  public DateTime CreatedTime { get; set; }

  [JsonPropertyName("url")]
  public string Url { get; set; }

  [JsonPropertyName("table_row")]
  public TableRow TableRow { get; set; }
}

public class TableRow
{
  [JsonPropertyName("cells")]
  public List<List<TableCell>> Cells { get; set; }
}

public class TableCell
{
  [JsonPropertyName("type")]
  public string Type { get; set; }

  [JsonPropertyName("plain_text")]
  public string PlainText { get; set; }
}