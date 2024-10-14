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
      return new CreatedCardDto(default, default, default, default);
    }
  }
}