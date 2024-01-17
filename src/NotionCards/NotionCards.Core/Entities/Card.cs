namespace NotionCards.Core.Entities;

public class Card
{
  public int Id { get; set; }
  public int SetId { get; set; }
  public string FrontText { get; set; }
  public string BackText { get; set; }

  public DateTime AddedTime { get; set; }

  public Set Set { get; set; }
}