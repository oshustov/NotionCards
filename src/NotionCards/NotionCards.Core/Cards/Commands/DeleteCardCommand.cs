using MediatR;
using Microsoft.EntityFrameworkCore;
using NotionCards.Storage;

namespace NotionCards.Core.Cards.Commands;

public class DeleteCardCommand : IRequest
{
  public int CardId { get; set; }

  public class Handler : IRequestHandler<DeleteCardCommand>
  {
    private readonly AppDbContext _appDbContext;

    public Handler(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
      var card = await _appDbContext.Cards.FirstOrDefaultAsync(x => x.Id == request.CardId, cancellationToken);
      if (card is null)
        throw new ArgumentException("There is no such card");

      _appDbContext.Cards.Remove(card);
      await _appDbContext.SaveChangesAsync(cancellationToken);
    }
  }
}