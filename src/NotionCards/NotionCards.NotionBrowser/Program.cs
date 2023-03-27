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
  await client.QueryDatabase();
});

app.Run();
