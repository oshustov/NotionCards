using MediatR;
using NotionCards.Core.Data.Entities;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;

namespace NotionCards.Core.Cards.Queries;

public class ListCardsQuery : IRequest<ListCardsResponseDto>
{
  public int SetId { get; set; }

  public string? NextPageToken { get; set; }
  public int? MaxCount { get; set; }
  public bool? Shuffle { get; set; }

  public class Handler : IRequestHandler<ListCardsQuery, ListCardsResponseDto>
  {
    private readonly ICardsStorage _cardsStorage;

    public Handler(ICardsStorage cardsStorage)
    {
      _cardsStorage = cardsStorage;
    }

    public async Task<ListCardsResponseDto> Handle(ListCardsQuery request, CancellationToken cancellationToken)
    {
      var token = request switch
      {
        { MaxCount: null, NextPageToken: null } => PaginationToken.All(),
        { MaxCount: not null, NextPageToken: null } => PaginationToken.Start(request.MaxCount.Value),
        { NextPageToken: not null } => PaginationToken.FromStringToken(request.NextPageToken)
      };

      var result = await _cardsStorage.ListBySetId(request.SetId, token);
      if (request.Shuffle is true)
        Random.Shared.Shuffle(result.Result);

      return new ListCardsResponseDto(result.NextToken?.GetString(),
        result.Result.Select(Selector).ToArray());
    }

    private CardDto Selector(Card x)
    {
      return new CardDto(x.Id, x.SetId, x.FrontText, x.BackText);
    }
  }
}