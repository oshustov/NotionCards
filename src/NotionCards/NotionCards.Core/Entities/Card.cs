namespace NotionCards.Core.Entities;

public class Card
{
  public int Id { get; set; }
  public SourceKind SourceKind { get; set; }
  public string FrontText { get; set; }
  public string BackText { get; set; }
  public DateTime AddedTime { get; set; }
}