// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.Text.Json.Serialization;

public class Annotations
{
  public bool bold { get; set; }
  public bool italic { get; set; }
  public bool strikethrough { get; set; }
  public bool underline { get; set; }
  public bool code { get; set; }
  public string color { get; set; }
}

public class Cover
{
  public string type { get; set; }
  public External external { get; set; }
}

public class CreatedBy
{
  public string @object { get; set; }
  public string id { get; set; }
}

public class Date
{
  public string id { get; set; }
  public string type { get; set; }
  public Date date { get; set; }
}

public class Date2
{
  public string start { get; set; }
  public object end { get; set; }
  public object time_zone { get; set; }
}

public class External
{
  public string url { get; set; }
}

public class Icon
{
  public string type { get; set; }
  public string emoji { get; set; }
}

public class LastEditedBy
{
  public string @object { get; set; }
  public string id { get; set; }
}

public class Name
{
  public string id { get; set; }
  public string type { get; set; }
  public List<Title> title { get; set; }
}

public class Page
{
}

public class Parent
{
  public string type { get; set; }
  public string database_id { get; set; }
}

public class PaymentDay
{
  public string id { get; set; }
  public string type { get; set; }
  public bool checkbox { get; set; }
}

public class Properties
{
  [JsonPropertyName("Payment day")]
  public PaymentDay Paymentday { get; set; }
  public Date Date { get; set; }
  public Name Name { get; set; }
}

public class Result
{
  public string @object { get; set; }
  public string type { get; set; }
  public string id { get; set; }
  public DateTime created_time { get; set; }
  public DateTime last_edited_time { get; set; }
  public CreatedBy created_by { get; set; }
  public LastEditedBy last_edited_by { get; set; }
  public Cover cover { get; set; }
  public Icon icon { get; set; }
  public Parent parent { get; set; }
  public bool archived { get; set; }
  public Properties properties { get; set; }
  public string url { get; set; }
  public bool has_children { get; set; }
  public TableRow table_row { get; set; }
}

public class TableRow
{
  public List<List<Cell>> cells { get; set; }
}

public class Cell
{
  public string type { get; set; }
  public Text text { get; set; }
  public Annotations annotations { get; set; }
  public string plain_text { get; set; }
  public string href { get; set; }
}

public class NotionResponse
{
  public string @object { get; set; }
  public List<Result> results { get; set; }
  public string next_cursor { get; set; }
  public bool has_more { get; set; }
  public string type { get; set; }
  public Page page { get; set; }
}

public class Text
{
  public string content { get; set; }
  public object link { get; set; }
}

public class Title
{
  public string type { get; set; }
  public Text text { get; set; }
  public Annotations annotations { get; set; }
  public string plain_text { get; set; }
  public object href { get; set; }
}

