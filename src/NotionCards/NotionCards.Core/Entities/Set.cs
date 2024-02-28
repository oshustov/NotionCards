namespace NotionCards.Core.Entities;

public class Set
{
  public int Id { get; set; }
  public string Name { get; set; }
  public DateTime Created { get; set; }

  public List<Card> Cards { get; set; } = new List<Card>();
}