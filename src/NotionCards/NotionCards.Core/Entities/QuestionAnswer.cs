namespace NotionCards.Core.Entities;

public class QuestionAnswer
{
  public int Id { get; set; }
  public LearningPlanQuestion LearnPlanQuestion { get; set; }
  public NotionDbEntry NotionDbEntry { get; set; }
  public bool IsCorrect { get; set; }
}