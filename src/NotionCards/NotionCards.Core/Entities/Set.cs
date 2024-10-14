namespace NotionCards.Core.Entities;

public class Set
{
  public int Id { get; set; }
  public string Name { get; set; }
  public DateTime CreatedAt { get; set; }

  public List<LeitnerBox> Boxes { get; set; }
}