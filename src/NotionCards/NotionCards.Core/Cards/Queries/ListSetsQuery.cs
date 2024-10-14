using MediatR;
using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Storage;

namespace NotionCards.Core.Cards.Queries;

public class ListSetsQuery : ListQuery, IRequest<IEnumerable<SetDto>>
{
  public class Handler : IRequestHandler<ListSetsQuery, IEnumerable<SetDto>>
  {
    private readonly AppDbContext _appDbContext;

    public Handler(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<SetDto>> Handle(ListSetsQuery request, CancellationToken cancellationToken)
    {
      var (maxCount, lastId, order) = request;

      var sets = await _appDbContext.Sets
        .
        ToListAsync(cancellationToken);

      return sets.Select(x => new SetDto(x.Id, x.Name, x.CreatedAt));
    }
  }
}