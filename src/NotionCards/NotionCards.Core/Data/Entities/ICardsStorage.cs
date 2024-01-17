using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Entities;
using NotionCards.Storage;
using NotionCards.Storage.Entities;

namespace NotionCards.Core.Data.Entities;

public interface ICardsStorage
{
  Task<PaginatedResponse<Card>> ListBySetId(int setId, PaginationToken token);
}

public class CardsStorage : ICardsStorage
{
  private readonly AppDbContext _dbContext;

  public CardsStorage(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<PaginatedResponse<Card>> ListBySetId(int setId, PaginationToken token)
  {
    var cards = token switch
    {
      { LastGivenId: null, MaxCount: null } =>
        await _dbContext.Cards
        .Where(x => x.SetId == setId)
        .OrderBy(x => x.AddedTime)
        .AsNoTracking()
        .ToListAsync(),

      { LastGivenId: not null, MaxCount: not null } =>
        await _dbContext.Cards
          .Where(x => x.SetId == setId && x.Id > token.LastGivenId.Value)
          .Take(token.MaxCount.Value + 1)
          .OrderBy(x => x.AddedTime)
          .AsNoTracking()
          .ToListAsync(),

      { MaxCount: > 0 } =>
        await _dbContext.Cards
        .Where(x => x.SetId == setId)
        .Take(token.MaxCount.Value + 1)
        .OrderBy(x => x.AddedTime)
        .AsNoTracking()
        .ToListAsync(),
    };

    if (cards is not { Count: > 0 })
      return new PaginatedResponse<Card>()
      {
        NextToken = null,
        Result = Array.Empty<Card>()
      };

    if (token.MaxCount is null)
      return new PaginatedResponse<Card>()
      {
        NextToken = null,
        Result = cards.ToArray()
      };

    var extraCard = cards.Last();
    var thereAreMore = cards.Count > token.MaxCount && token.MaxCount.HasValue;
    if (thereAreMore)
      cards.Remove(extraCard);

    return new PaginatedResponse<Card>()
    {
      NextToken = token.ContinueFrom(cards.Last().Id),
      Result = cards.ToArray()
    };
  }
}