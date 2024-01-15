using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Entities;
using NotionCards.Storage;

namespace NotionCards.Core.Services;

public interface ISetCardsSource
{
  Task<Card[]> GetCards();
}

public class NotionSetCardsSource : ISetCardsSource
{
  public TimeSpan Period { get; set; } = TimeSpan.FromDays(1);
  public AppDbContext DbContext { get; set; }

  public NotionSetCardsSource(AppDbContext dbContext) => 
    DbContext = dbContext;

  public async Task<Card[]> GetCards()
  {
    var lastAddedDate = await DbContext.NotionDbRecords
      .OrderByDescending(x => x.DateAdded)
      .FirstOrDefaultAsync();

    var notionRecords = await DbContext.NotionDbRecords
      .OrderByDescending(x => x.DateAdded)
      .Where(x => x.DateAdded >= lastAddedDate.DateAdded.Subtract(Period))
      .ToListAsync();

    var cards = notionRecords.Select(x => new Card()
    {
      FrontText = x.Text,
      BackText = x.Translation
    }).ToArray();

    return cards;
  }
}