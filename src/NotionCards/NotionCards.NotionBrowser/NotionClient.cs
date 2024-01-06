using Microsoft.Extensions.Options;
using Notion.Client;
using NotionCards.Storage;

namespace NotionCards.NotionBrowser
{
  public class NotionDbEntry
  {
    public NotionDbEntry(string Expression, string Translation, DateTime AddedToNotion, string PageId, DateTime AddedAt, DateTime UpdatedAt)
    {
      this.Expression = Expression;
      this.Translation = Translation;
      this.AddedToNotion = AddedToNotion;
      this.PageId = PageId;
      this.AddedAt = AddedAt;
      this.UpdatedAt = UpdatedAt;
    }

    public string Expression { get; init; }
    public string Translation { get; init; }
    public DateTime AddedToNotion { get; init; }
    public string PageId { get; init; }
    public DateTime AddedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
  }

  public class NotionClient
  {
    private readonly NotionOptions _options;
    private AppDbContext _appDbContext;

    public NotionClient(IOptions<NotionOptions> options, AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
      _options = options.Value;
    }

    public async Task ReadEntireDb()
    {
      var client = NotionClientFactory.Create(new ClientOptions()
      {
        AuthToken = _options.NotionSecretId
      });

      var parameters = new DatabasesQueryParameters()
      {
        PageSize = 100,
        StartCursor = null
      };

      var translationKey = "Meaning/translation";
      var expressionKey = "Expression";
      var addedKey = "Date";

      do
      {
        var pages = await client.Databases.QueryAsync(_options.DatabaseId, parameters);

        var data = pages.Results.Select(x =>
        {
          var expression = string.Empty;
          var translation = string.Empty;
          var addedAt = DateTime.MinValue;
          var addedAndUpdated = DateTime.UtcNow;

          if (x.Properties.TryGetValue(expressionKey, out var expressionProp))
            expression = TextValue(expressionProp);

          if (x.Properties.TryGetValue(translationKey, out var translationProp))
            translation = TextValue(translationProp);

          if (x.Properties.TryGetValue(addedKey, out var dateProp))
            addedAt = dateProp is DatePropertyValue datePropValue ? datePropValue.Date.Start ?? datePropValue.Date.End ?? default : default;

          return new NotionDbEntry(expression, translation, addedAt, x.Id, addedAndUpdated, addedAndUpdated);
        }).ToList();

        await _appDbContext.AddRangeAsync(data);

        parameters.StartCursor = pages.HasMore
          ? pages.NextCursor
          : null;
      }
      while (parameters.StartCursor != null);

      await _appDbContext.SaveChangesAsync()
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
  }
}
