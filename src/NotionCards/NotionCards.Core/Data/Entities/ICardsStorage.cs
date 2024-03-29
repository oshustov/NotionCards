﻿using Microsoft.EntityFrameworkCore;
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
    var fetchedCards = token switch
    {
      { LastId: null, MaxCount: null } =>
        await _dbContext.Cards
        .Where(x => x.SetId == setId)
        .AsNoTracking()
        .ToListAsync(),

      { LastId: not null, MaxCount: not null } =>
        await _dbContext.Cards
          .Where(x => x.SetId == setId && x.Id > token.LastId.Value)
          .Take(token.MaxCount.Value + 1)
          .AsNoTracking()
          .ToListAsync(),

      { MaxCount: > 0 } =>
        await _dbContext.Cards
        .Where(x => x.SetId == setId)
        .Take(token.MaxCount.Value + 1)
        .AsNoTracking()
        .ToListAsync(),
    };

    if (fetchedCards is not { Count: > 0 })
      return new PaginatedResponse<Card>()
      {
        NextToken = null,
        Result = Array.Empty<Card>()
      };

    if (token.MaxCount is null)
      return new PaginatedResponse<Card>()
      {
        NextToken = null,
        Result = fetchedCards.ToArray()
      };
    
    var thereAreMore = token.MaxCount.HasValue && fetchedCards.Count > token.MaxCount;
    if (thereAreMore)
      fetchedCards.Remove(fetchedCards.Last());

    return new PaginatedResponse<Card>()
    {
      NextToken = token.ContinueFrom(fetchedCards.Last().Id),
      Result = fetchedCards.ToArray()
    };
  }
}