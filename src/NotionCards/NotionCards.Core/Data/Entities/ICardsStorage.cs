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
    return new PaginatedResponse<Card>();
  }
}