namespace NotionCards.Core.Entities;

public class NotionDbSetup
{
  public string NotionDbId { get; set; }
  public string FieldMappings { get; set; }
  public string NotionDbName { get; set; }
  public TimeSpan? PullingInterval { get; set; } = TimeSpan.FromDays(1);
}