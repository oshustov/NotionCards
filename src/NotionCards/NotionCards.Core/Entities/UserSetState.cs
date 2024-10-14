namespace NotionCards.Core.Entities;

public class UserSetState
{
  public string UserId { get; set; }
  public int SetId { get; set; }
  public int LastRequest { get; set; }
}