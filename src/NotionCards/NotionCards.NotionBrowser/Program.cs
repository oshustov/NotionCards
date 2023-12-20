using Microsoft.AspNetCore.Mvc;
using NotionCards.NotionBrowser;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<NotionOptions>(builder.Configuration);
builder.Services.AddSingleton<NotionClient>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", async ([FromServices] NotionClient client) =>
{
  await client.ReadEntireDb();
});

app.Run();