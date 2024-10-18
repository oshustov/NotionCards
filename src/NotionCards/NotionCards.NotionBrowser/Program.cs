using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Notion.Client;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Core.Notion;
using NotionCards.NotionBrowser.Endpoints;
using NotionCards.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Card>());
builder.Services.Configure<NotionOptions>(builder.Configuration);
builder.Services.AddScoped<NotionClientAdapter>();
builder.Services.AddStorage();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<NotionClient>((x) => NotionClientFactory.Create(new ClientOptions()
{
  AuthToken = x.GetRequiredService<IOptions<NotionOptions>>().Value.NotionSecretId
}));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(x =>
{
  x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
  x.RoutePrefix = "swagger";
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGroup("/sets").MapSetEndpoints().WithTags("Sets");
app.MapGroup("/cards").MapCardsEndpoints().WithTags("Cards");

app.MapPost("/notion:import", async ([FromServices] NotionClientAdapter client) =>
{
  await client.ReadEntireDb();
});

app.MapFallbackToFile("index.html");

app.Run();