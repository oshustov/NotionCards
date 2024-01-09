namespace NotionCards.Core.Entities
{
  public class NotionDbRecord
  {
    public int Id { get; set; }
    public string NotionId { get; set; }
    public string Text { get; set; }
    public string Translation { get; set; }
    public EntryType Type { get; set; }
    public DateTime DateAdded { get; set; }
  }

  public enum EntryType
  {
    Expression = 1,
    Word = 2
  }
}
