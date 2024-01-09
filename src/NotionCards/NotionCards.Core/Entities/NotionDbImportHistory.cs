namespace NotionCards.Core.Entities;

public class NotionDbImportHistory
{
  public int Id { get; set; }
  public string NotionDbId { get; set; }
  public DateTime LastOperation { get; set; }
}