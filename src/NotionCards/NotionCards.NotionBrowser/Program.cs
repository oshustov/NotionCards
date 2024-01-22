using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Data.Entities;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Core.Notion;
using NotionCards.Core.Services;
using NotionCards.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<NotionOptions>(builder.Configuration);
builder.Services.AddScoped<NotionClient>();
builder.Services.AddTransient<ICardsSource, NotionCardsSource>();
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

app.MapPost("/sets/{setId:int}/cards:populate-with-notion", async ([FromRoute] int setId, [FromBody] PopulateWithNotionDto dto, [FromServices] IEnumerable<ICardsSource> sources, [FromServices] AppDbContext dbContext) =>
{
  var set = await dbContext.Sets.FirstOrDefaultAsync(x => x.Id == setId);
  if (set == null)
    return Results.BadRequest("The set doesn't exist");

  var source = sources.FirstOrDefault(x => x is NotionCardsSource);
  if (source == null)
    return Results.BadRequest("No Notion source is configured.");
  
  var cards = await source.GetCards(dto);
  set.Cards.AddRange(cards);
  await dbContext.SaveChangesAsync();

  return Results.Ok(cards.Select(x => new CardCreatedDto(x.Id, x.SetId, x.FrontText, x.BackText)));
});

app.MapPost("sets/{setId:int}/cards:list", async ([FromRoute] int setId, [FromBody] ListCardsDto dto, [FromServices] ICardsStorage storage) =>
{
  var token = dto switch
  {
    {MaxCount: null, NextToken: null} => PaginationToken.All(),
    {MaxCount: not null, NextToken: null} => PaginationToken.Start(dto.MaxCount.Value),
    {NextToken: not null} => PaginationToken.FromStringToken(dto.NextToken)
  };

  var result = await storage.ListBySetId(setId, token);
  if (dto.Shuffled)
    Random.Shared.Shuffle(result.Result);

  return Results.Ok(new ListCardsResponseDto(result.NextToken?.GetString(), result.Result.Select(x => new CardDto(x.Id, x.SetId, x.FrontText, x.BackText)).ToArray()));
});

app.MapPut("/cards/{cardId:int}", async ([FromRoute] int cardId, [FromBody] ChangeCardDto dto, [FromServices] AppDbContext dbContext) =>
{
  var card = await dbContext.Cards.FirstOrDefaultAsync(x => x.Id == cardId);
  if (card is null)
    return Results.BadRequest("There is no such card");

  card.FrontText = dto.FrontText;
  card.BackText = dto.BackText;

  await dbContext.SaveChangesAsync();
  return Results.Ok(new CardCreatedDto(card.Id, card.SetId, card.FrontText, card.BackText));
});

app.MapPost("/cards", async ([FromBody] CreateCardDto dto, [FromServices] AppDbContext dbContext) =>
{
  var set = await dbContext.Sets.FirstOrDefaultAsync(x => x.Id == dto.SetId);
  if (set is null)
    return Results.BadRequest("There is no such set");

  var card = new Card()
  {
    SetId = dto.SetId,
    FrontText = dto.FrontText,
    BackText = dto.BackText
  };

  set.Cards.Add(card);
  await dbContext.SaveChangesAsync();

  return Results.Ok(new CardCreatedDto(card.Id, card.SetId, card.FrontText, card.BackText));
});

app.MapDelete("/cards/{cardId:int}", async ([FromRoute] int cardId, [FromServices] AppDbContext dbContext) =>
{
  var card = await dbContext.Cards.FirstOrDefaultAsync(x => x.Id == cardId);
  if (card is null)
    return Results.BadRequest("There is no such card");

  dbContext.Remove(card);
  await dbContext.SaveChangesAsync();

  return Results.Ok();
});

app.Run();