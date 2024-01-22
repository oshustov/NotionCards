using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Cards.Commands;

namespace NotionCards.NotionBrowser.Endpoints;

public static partial class RouteGroupBuilderExtensions
{
  public static RouteGroupBuilder MapCardsEndpoints(this RouteGroupBuilder builder)
  {
    builder.MapPut("{cardId}", UpdateCard);
    builder.MapDelete("{cardId}", DeleteCard);

    return builder;
  }

  private static async Task<IResult> UpdateCard([FromRoute] int cardId, [FromBody] UpdateCardCommand command, [FromServices] ISender sender)
  {
    command.CardId = cardId;
    var updatedCard = await sender.Send(command);
    return Results.Ok(updatedCard);
  }

  private static async Task<IResult> DeleteCard([FromRoute] int cardId, [FromBody] DeleteCardCommand command, [FromServices] ISender sender)
  {
    command.CardId = cardId;
    await sender.Send(command);

    return Results.Ok();
  }
}