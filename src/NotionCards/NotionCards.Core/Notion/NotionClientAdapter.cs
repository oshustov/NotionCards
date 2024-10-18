using System.Collections.Frozen;
using System.Text.Json;
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

  public class NotionClientAdapter
  {
    private readonly NotionOptions _options;
    private readonly AppDbContext _appDbContext;
    private readonly NotionClient _client;

    public NotionClientAdapter(IOptions<NotionOptions> options, AppDbContext appDbContext, NotionClient client)
    {
      _appDbContext = appDbContext;
      _client = client;
      _options = options.Value;
    }

    public async Task ReadEntireDb(string requestNotionDbId)
    {
      var lastUpdate = _appDbContext.NotionDbPulls.FirstOrDefault(x => x.NotionDbId == _options.DatabaseId);

      var fetchForPeriod = lastUpdate?.LastRecordDateTime ?? DateTime.MinValue;
      var parameters = new DatabasesQueryParameters()
      {
        PageSize = 100,
        StartCursor = null,
        Filter = new DateFilter("Date", onOrAfter: fetchForPeriod),
        Sorts = [new() { Direction = Direction.Descending, Timestamp = Timestamp.CreatedTime }]
      };

      var dbSetup = await _appDbContext.NotionDbs.FindAsync(requestNotionDbId);
      if (dbSetup is null)
        throw new InvalidOperationException("There is no database setup for that operation.");

      var mappings = JsonSerializer.Deserialize<NotionDbFieldMappings>(dbSetup.FieldMappings);

      DateTime lastRecordDateTime;
      do
      {
        var pages = await _client.Databases.QueryAsync(_options.DatabaseId, parameters);

        var parsedPages = pages.Results.Select(x =>
        {
          var properties = new List<NotionField>();

          foreach (var property in x.Properties) 
            properties.Add(MapToNotionField(property));

          return new NotionPage(x.Id, properties.ToFrozenDictionary(y => y.Name, y => y));
        }).ToList();

        var records = MapToCards(parsedPages, mappings);
        lastRecordDateTime = records.First().AddedTime;

        await _appDbContext.Cards.AddRangeAsync(records);

        parameters.StartCursor = pages.HasMore
          ? pages.NextCursor
          : null;
      }
      while (parameters.StartCursor != null);

      if (lastUpdate is null)
        await _appDbContext.NotionDbPulls.AddAsync(new NotionDbPull()
        {
          LastRecordDateTime = lastRecordDateTime,
          NotionDbId = _options.DatabaseId
        });
      else
        lastUpdate.LastRecordDateTime = DateTime.UtcNow;

      await _appDbContext.SaveChangesAsync();
    }

    private List<Card> MapToCards(List<NotionPage> fields, NotionDbFieldMappings mappings)
    {
      return fields.Select(x => new Card()
      {
        AddedTime = DateTime.Parse(x.FieldsByName.GetValueOrDefault("Date")!.Value),
        FrontText = x.FieldsByName.GetValueOrDefault(mappings.FrontTextField)!.Value,
        BackText = x.FieldsByName.GetValueOrDefault(mappings.BackTextField)!.Value
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
