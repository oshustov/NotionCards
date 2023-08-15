using Marten;
using Microsoft.Extensions.Options;
using Notion.Client;
using NotionCards.Core.Entities;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace NotionCards.NotionBrowser
{
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

      var expressions = new ConcurrentBag<LangExpression>();

      do
      {
        var pages = await client.Databases.QueryAsync(_options.DatabaseId, parameters);
        var tasks = pages.Results
          .Select(x => ReadPageRows(client, x)
            .ContinueWith(x => x.Result.ForEach(y => expressions.Add(y))));

        await Task.WhenAll(tasks);

        parameters.StartCursor = pages.HasMore
          ? pages.NextCursor
          : null;
      }
      while (parameters.StartCursor != null);

      var count = expressions.Count;
    }

    private static async Task<List<LangExpression>> ReadPageRows(Notion.Client.NotionClient client, Page page)
    {
      var expressions = new List<LangExpression>();

      var dbPageBlocks = await client.Blocks.RetrieveChildrenAsync(page.Id);
      var table = dbPageBlocks.Results.FirstOrDefault(x => x.Type == BlockType.Table);
      if (table == null)
        return expressions;

      var tableRows = await client.Blocks.RetrieveChildrenAsync(table.Id);
      if (tableRows == null)
        return expressions;

      var tableRowBlocks = tableRows.Results.Cast<TableRowBlock>().ToList();

      // i = 1 because the first row is always a heading row
      for (var i = 1; i < tableRowBlocks.Count; i++)
      {
        var cells = tableRowBlocks[i].TableRow.Cells.ToList();
        if (cells is not { Count: > 1 })
          continue;

        var invariant = cells[0].FirstOrDefault()?.PlainText;
        var translation = cells[1].FirstOrDefault()?.PlainText;

        if (invariant == null && translation == null)
          continue;

        expressions.Add(new LangExpression
        {
          Invariant = invariant,
          Translation = translation
        });
      }

      return expressions;
    }
  }
}
