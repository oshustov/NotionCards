using MediatR;
using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Dto;
using NotionCards.Storage;

namespace NotionCards.Core.Cards.Commands;

public class UpdateCardCommand : IRequest<CreatedCardDto>
{
  public int CardId { get; set; }
  public string? FrontText { get; set; }
  public string? BackText { get; set; }

  public class Handler : IRequestHandler<UpdateCardCommand, CreatedCardDto>
  {
    private readonly AppDbContext _appDbContext;

    public Handler(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task<CreatedCardDto> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
      var card = await _appDbContext.Cards.FirstOrDefaultAsync(x => x.Id == request.CardId, cancellationToken);
      if (card is null) 
        throw new ArgumentException("There is no such card");

      card.FrontText = request.FrontText ?? card.FrontText;
      card.BackText = request.BackText ?? card.BackText;

      await _appDbContext.SaveChangesAsync(cancellationToken);

      return new CreatedCardDto(card.Id, card.SetId, card.FrontText, card.BackText);
    }
  }
}