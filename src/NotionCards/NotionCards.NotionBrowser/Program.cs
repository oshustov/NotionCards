using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Core.Notion;
using NotionCards.Core.Services;
using NotionCards.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<NotionOptions>(builder.Configuration);
builder.Services.AddScoped<NotionClient>();
builder.Services.AddTransient<ISetCardsSource, NotionSetCardsSource>();
builder.Services.AddStorage();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(x =>
{
  x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
  x.RoutePrefix = string.Empty;
});

app.MapPost("/notion:import", async ([FromServices] NotionClient client) =>
{
  await client.ReadEntireDb();
});

app.MapPost("/sets", async ([FromServices] AppDbContext dbContext, [FromBody] CreateSetDto dto) =>
{
  var set = new Set()
  {
    Name = dto.Name
  };

  await dbContext.Sets.AddAsync(set);
  await dbContext.SaveChangesAsync();
});

app.MapPost("/sets/{setId:int}/cards:populate-with-notion", async ([FromServices] IEnumerable<ISetCardsSource> sources, [FromRoute] int setId, [FromServices] AppDbContext dbContext) =>
{
  var set = await dbContext.Sets.FirstOrDefaultAsync(x => x.Id == setId);
  if (set == null)
    return Results.BadRequest("The set doesn't exist");

  var source = sources.FirstOrDefault(x => x is NotionSetCardsSource);
  if (source == null)
    return Results.BadRequest("No Notion source is configured.");

  var cards = await source.GetCards();
  set.Cards = new List<Card>();
  set.Cards.AddRange(cards);
  await dbContext.SaveChangesAsync();

  return Results.Ok(cards.Select(x => new CardCreatedDto(x.Id, x.SetId, x.FrontText, x.BackText)));
});

app.Run();