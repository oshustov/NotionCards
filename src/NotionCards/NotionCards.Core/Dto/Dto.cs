namespace NotionCards.Core.Dto;

public record CreateSetDto(string Name);

public record CardCreatedDto(int Id, int SetId, string FrontText, string BackText);