namespace NotionCards.Core.Entities;

public class NotionDbRecord
{
  public int Id { get; set; }
  public string NotionId { get; set; }
  public string NotionDbId { get; set; }
  public string FrontText { get; set; }
  public string BackText { get; set; }
  public DateTime DateAdded { get; set; }
}