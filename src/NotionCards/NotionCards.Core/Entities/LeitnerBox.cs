namespace NotionCards.Core.Entities;

public class LeitnerBox
{
  public int Id { get; set; }
  public int Frequency { get; set; }
  public int OrderNumber { get; set; }
  public int SetId { get; set; }

  public Set Set { get; set; }
}