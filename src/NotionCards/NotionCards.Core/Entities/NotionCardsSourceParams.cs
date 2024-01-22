namespace NotionCards.Core.Entities;

public enum NotionRecordsRange
{
  LastWeek,
  LastMonth,
  All,
  ExactDatesRange,
  Top50,
  Top100
};

public record NotionCardSourceParams(DateTime? MinDate, DateTime? MaxDate, NotionRecordsRange Range);