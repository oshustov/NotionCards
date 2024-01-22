namespace NotionCards.Core.Dto;

public record CreateSetDto(string Name);

public record CardCreatedDto(int Id, int SetId, string FrontText, string BackText);

public record ChangeCardDto(string FrontText, string BackText);

public record CreateCardDto(int SetId, string FrontText, string BackText);

public record CardDto(int CardId, int SetId, string FrontText, string BackText);

public record PopulateWithNotionDto(DateTime? MinDate, DateTime? MaxDate);

public record ListCardsDto(string? NextToken, int? MaxCount, bool Shuffled = false);
public record ListCardsResponseDto(string? NextToken, CardDto[] Cards);