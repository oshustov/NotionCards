using Microsoft.EntityFrameworkCore;
using Notion.Client;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Storage;

namespace NotionCards.Core.Services;

public interface ICardsSource
{
  Task<Card[]> GetCards(NotionCardSourceParams parameters);
}

public class NotionCardsSource : ICardsSource
{
  public AppDbContext DbContext { get; set; }

  public NotionCardsSource(AppDbContext dbContext) => 
    DbContext = dbContext;

  public async Task<Card[]> GetCards(NotionCardSourceParams parameters)
  {

    return parameters.Range switch
    {
      NotionRecordsRange.ExactDatesRange => await ExactDatesRangeCards(parameters.MinDate, parameters.MaxDate),
      NotionRecordsRange.LastMonth => await ExactDatesRangeCards(DateTime.UtcNow.AddMonths(-1), maxDate: null),
      NotionRecordsRange.LastWeek => await ExactDatesRangeCards(DateTime.UtcNow.AddDays(-7), maxDate: null),
      NotionRecordsRange.Top100 => await TopCards(top: 100),
      NotionRecordsRange.Top50 => await TopCards(top: 50),
      NotionRecordsRange.All => await ExactDatesRangeCards(minDate:null, maxDate:null)
    };
  }

  private async Task<Card[]> TopCards(int top)
  {
    var records = await DbContext.NotionDbRecords
      .OrderByDescending(x => x.DateAdded)
      .Take(top)
      .ToListAsync();

    return records.Select(ToCard).ToArray();
  }

  private async Task<Card[]> ExactDatesRangeCards(DateTime? minDate, DateTime? maxDate)
  {
    IQueryable<NotionDbRecord> records = DbContext.NotionDbRecords;

    if (minDate.HasValue)
      records = records.Where(x => x.DateAdded >= minDate.Value);

    if (maxDate.HasValue)
      records = records.Where(x => x.DateAdded <= maxDate.Value);

    var notionRecords = await records.ToListAsync();

    var cards = notionRecords.Select(ToCard).ToArray();

    return cards;
  }

  private Card ToCard(NotionDbRecord x)
  {
    return new Card() {FrontText = x.Text, BackText = x.Translation};
  }
}