using Microsoft.AspNetCore.Mvc;
using NotionCards.NotionBrowser;
using NotionCards.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<NotionOptions>(builder.Configuration);
builder.Services.AddSingleton<NotionClient>();
builder.Services.AddStorage();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", async ([FromServices] NotionClient client) =>
{
  await client.ReadEntireDb();
});

app.Run();