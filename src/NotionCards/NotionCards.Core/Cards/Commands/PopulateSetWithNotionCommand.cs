using MediatR;
using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Core.Services;
using NotionCards.Storage;

namespace NotionCards.Core.Cards.Commands;

public class PopulateSetWithNotionCommand : IRequest<IEnumerable<CreatedCardDto>>
{
  public int SetId { get; set; }
  public DateTime? MinDate { get; set; }
  public DateTime? MaxDate { get; set; }
  public NotionRecordsRange Range { get; set; }

  public class Handler: IRequestHandler<PopulateSetWithNotionCommand, IEnumerable<CreatedCardDto>>
  {
    private readonly AppDbContext _appDbContext;
    private readonly IEnumerable<ICardsSource> _cardsSources;

    public Handler(AppDbContext appDbContext, IEnumerable<ICardsSource> cardsSources)
    {
      _appDbContext = appDbContext;
      _cardsSources = cardsSources;
    }

    public async Task<IEnumerable<CreatedCardDto>> Handle(PopulateSetWithNotionCommand request, CancellationToken cancellationToken)
    {
      var set = await _appDbContext.Sets.FirstOrDefaultAsync(x => x.Id == request.SetId, cancellationToken);
      if (set == null)
        throw new ArgumentException("The set doesn't exist");

      var source = _cardsSources.FirstOrDefault(x => x is NotionCardsSource);
      if (source == null)
        throw new ArgumentException("No Notion source is configured.");

      var cards = await source.GetCards(new NotionCardSourceParams(request.MinDate, request.MaxDate, request.Range));

      await _appDbContext.SaveChangesAsync(cancellationToken);

      return cards.Select(x => new CreatedCardDto(x.Id, default, x.FrontText, x.BackText));
    }
  }
}