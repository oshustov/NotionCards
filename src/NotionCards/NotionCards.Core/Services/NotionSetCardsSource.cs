using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Storage;

namespace NotionCards.Core.Services;

public interface ICardsSource
{
  Task<Card[]> GetCards(PopulateWithNotionDto parameters);
}

public class NotionCardsSource : ICardsSource
{
  public AppDbContext DbContext { get; set; }

  public NotionCardsSource(AppDbContext dbContext) => 
    DbContext = dbContext;

  public async Task<Card[]> GetCards(PopulateWithNotionDto parameters)
  {
    IQueryable<NotionDbRecord> records = DbContext.NotionDbRecords;

    if (parameters.MinDate.HasValue)
      records = records.Where(x => x.DateAdded >= parameters.MinDate.Value);

    if (parameters.MaxDate.HasValue)
      records = records.Where(x => x.DateAdded <= parameters.MaxDate.Value);

    var notionRecords = await records.ToListAsync();

    var cards = notionRecords.Select(x => new Card()
    {
      FrontText = x.Text,
      BackText = x.Translation
    }).ToArray();

    return cards;
  }
}