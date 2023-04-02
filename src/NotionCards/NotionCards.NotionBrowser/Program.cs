using NotionCards.NotionBrowser;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<NotionOptions>(builder.Configuration);
builder.Services.AddHttpClient<NotionClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("api/notion", async (HttpContext context, NotionClient client) =>
{
  var pages = await client.FetchDbPages(5);

  foreach (var x in pages.Results)
  {
    var pageChildren = await client.FetchBlocks(x.Id);
    var tableBlock = pageChildren.Results.FirstOrDefault(x => x.Type == "table");
    if (tableBlock is null)
      continue;

    var tableContent = await client.FetchBlocks(tableBlock.Id);
  }
});

app.Run();
