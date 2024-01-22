using MediatR;
using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Storage;

namespace NotionCards.Core.Cards.Commands;

public class CreateCardCommand : IRequest<CreatedCardDto>
{
  public int SetId { get; set; }
  public string FrontText { get; set; }
  public string BackText { get; set; }

  public class Handler : IRequestHandler<CreateCardCommand, CreatedCardDto>
  {
    private readonly AppDbContext _appDbContext;

    public Handler(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task<CreatedCardDto> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
      var set = await _appDbContext.Sets.FirstOrDefaultAsync(x => x.Id == request.SetId, cancellationToken);
      if (set is null) 
        throw new ArgumentException("There is no such set");

      var card = new Card()
      {
        SetId = request.SetId,
        FrontText = request.FrontText,
        BackText = request.BackText
      };

      set.Cards.Add(card);
      await _appDbContext.SaveChangesAsync(cancellationToken);

      return new CreatedCardDto(card.Id, card.SetId, card.FrontText, card.BackText);
    }
  }
}