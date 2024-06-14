using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NotionCards.Core.Cards.Commands;
using NotionCards.Core.Cards.Queries;
using NotionCards.Core.Dto;

namespace NotionCards.NotionBrowser.Endpoints;

public static partial class RouteGroupBuilderExtensions
{
  public static RouteGroupBuilder MapSetEndpoints(this RouteGroupBuilder builder)
  {
    builder.MapPost("/", CreateSet);
    builder.MapPost("{setId:int}/cards:list", ListCards);
    builder.MapPost("/{setId:int}/cards:populate-with-notion", PopulateSetWithNotion);
    builder.MapPost("/{setId:int}/cards", CreateCard);
    builder.MapGet("/", ListSets);

    return builder;
  }

  private static async Task<IResult> ListSets([FromServices] ISender sender)
  {
    return Results.Ok();
  }

  private static async Task<IResult> CreateSet([FromBody] CreateSetCommand command, [FromServices] ISender sender)
  {
    var result = await sender.Send(command);
    return Results.Ok(result);
  }

  private static async Task<IResult> PopulateSetWithNotion([FromRoute] int setId, [FromBody] PopulateSetWithNotionCommand command, [FromServices] ISender sender)
  {
    command.SetId = setId;
    var createdCards = await sender.Send(command);
    return Results.Ok(createdCards);
  }

  private static async Task<IResult> ListCards([FromRoute] int setId, [FromBody] ListCardsDto dto, [FromServices] ISender sender)
  {
    var query = new ListCardsQuery()
    {
      NextPageToken = dto.NextToken,
      SetId = setId,
      MaxCount = dto.MaxCount,
      Shuffle = dto.Shuffled
    };

    var response = await sender.Send(query);
    return Results.Ok(response);
  }

  private static async Task<IResult> CreateCard([FromRoute] int setId, [FromBody] CreateCardCommand command, [FromServices] ISender sender)
  {
    var createdCard = await sender.Send(command);
    return Results.Ok(createdCard);
  }
}