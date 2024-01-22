using MediatR;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Storage;

namespace NotionCards.Core.Cards.Commands;

public class CreateSetCommand : IRequest<CreatedSetDto>
{
  public string Name { get; set; }

  public class Handler : IRequestHandler<CreateSetCommand, CreatedSetDto>
  {
    private readonly AppDbContext _appDbContext;

    public Handler(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task<CreatedSetDto> Handle(CreateSetCommand request, CancellationToken cancellationToken)
    {
      var newSet = new Set()
      {
        Name = request.Name,
      };

      await _appDbContext.Sets.AddAsync(newSet, cancellationToken);
      await _appDbContext.SaveChangesAsync(cancellationToken);

      return new CreatedSetDto(newSet.Id, newSet.Name);
    }
  }
}