using Microsoft.Extensions.Options;
using Notion.Client;
using NotionCards.Core.Entities;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace NotionCards.NotionBrowser
{
  public record NotionDbEntry(string Expression, string Translation, DateTime AddedToNotion, string PageId, DateTime AddedAt, DateTime UpdatedAt);

  public class NotionClient
  {
    private readonly NotionOptions _options;

    public NotionClient(IOptions<NotionOptions> options)
    {
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

        parameters.StartCursor = pages.HasMore
          ? pages.NextCursor
          : null;
      }
      while (parameters.StartCursor != null);
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
