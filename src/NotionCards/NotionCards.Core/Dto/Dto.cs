namespace NotionCards.Core.Dto;

public record CreatedSetDto(int Id, string Name);
public record CreatedCardDto(int Id, int SetId, string FrontText, string BackText);

public record CardDto(int CardId, int SetId, string FrontText, string BackText);
public record SetDto(int Id, string Name, DateTime Created);

public record ListCardsDto(string? NextToken, int? MaxCount, bool? Shuffled = false);
public record ListCardsResponseDto(string? NextToken, CardDto[] Cards);