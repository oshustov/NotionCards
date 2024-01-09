using System.Collections.Frozen;
using Microsoft.Extensions.Options;
using Notion.Client;
using NotionCards.Core.Entities;
using NotionCards.Storage;

namespace NotionCards.Core.Notion
{
  public enum NotionFieldType
  {
    Unknown = 0,
    Text,
    Date
  }

  public record NotionField(string Name, string Value, NotionFieldType Type);

  public record NotionPage(string NotionId, FrozenDictionary<string, NotionField> FieldsByName);

  public class NotionClient
  {
    private readonly NotionOptions _options;
    private readonly AppDbContext _appDbContext;

    public NotionClient(IOptions<NotionOptions> options, AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
      _options = options.Value;
    }

    public async Task ReadEntireDb()
    {
      var client = NotionClientFactory.Create(new ClientOptions { AuthToken = _options.NotionSecretId });

      var lastUpdate = _appDbContext.ImportHistories.FirstOrDefault(x => x.NotionDbId == _options.DatabaseId);

      var fetchForPeriod = lastUpdate?.LastOperation ?? DateTime.MinValue;
      var parameters = new DatabasesQueryParameters()
      {
        PageSize = 100,
        StartCursor = null,
        Filter = new DateFilter("Date", onOrAfter: fetchForPeriod),
        Sorts = new List<Sort>()
        {
          new() { Direction = Direction.Descending, Timestamp = Timestamp.CreatedTime }
        }
      };

      do
      {
        var pages = await client.Databases.QueryAsync(_options.DatabaseId, parameters);

        var parsedPages = pages.Results.Select(x =>
        {
          var properties = new List<NotionField>();

          foreach (var property in x.Properties) 
            properties.Add(MapToNotionField(property));

          return new NotionPage(x.Id, properties.ToFrozenDictionary(y => y.Name, y => y));
        }).ToList();

        var records = MapToNotionDbRecords(parsedPages);
        await _appDbContext.NotionDbRecords.AddRangeAsync(records);

        parameters.StartCursor = pages.HasMore
          ? pages.NextCursor
          : null;
      }
      while (parameters.StartCursor != null);

      if (lastUpdate is null)
        await _appDbContext.ImportHistories.AddAsync(new NotionDbImportHistory()
        {
          LastOperation = DateTime.UtcNow,
          NotionDbId = _options.DatabaseId
        });
      else
        lastUpdate.LastOperation = DateTime.UtcNow;

      await _appDbContext.SaveChangesAsync();
    }

    private List<NotionDbRecord> MapToNotionDbRecords(List<NotionPage> fields)
    {
      return fields.Select(x => new NotionDbRecord()
      {
        NotionId = x.NotionId,
        Type = EntryType.Expression,
        DateAdded = DateTime.Parse(x.FieldsByName.GetValueOrDefault("Date")!.Value),
        Text = x.FieldsByName.GetValueOrDefault("Expression")!.Value,
        Translation = x.FieldsByName.GetValueOrDefault("Meaning/translation")!.Value
      }).ToList();
    }

    private NotionField MapToNotionField(KeyValuePair<string, PropertyValue> property)
    {
      var value = property.Value switch
      {
        RichTextPropertyValue y => (TextValue(y), NotionFieldType.Text),
        TitlePropertyValue y => (TextValue(y), NotionFieldType.Text),
        DatePropertyValue y => (DateUtcValue(y), NotionFieldType.Date),
        _ => ($"unknown property type {property.Value.Type}", NotionFieldType.Unknown)
      };

      var field = new NotionField(property.Key, value.Item1, value.Item2);
      return field;
    }

    private string TextValue(PropertyValue? notionProperty)
    {
      return notionProperty switch
      {
        TitlePropertyValue x => x.Title.FirstOrDefault()?.PlainText ?? "no text were found in title property value",
        RichTextPropertyValue x => x.RichText.FirstOrDefault()?.PlainText ?? "no text were found in rich text property value",
        null => "null",
        _ => $"no handler for {notionProperty.Type} was found"
      };
    }

    private string DateUtcValue(DatePropertyValue value)
    {
      return (value.Date.Start?.ToUniversalTime() ?? value.Date.End!.Value.ToUniversalTime()).ToString();
    }
  }
}
