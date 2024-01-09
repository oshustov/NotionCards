namespace NotionCards.Core.Entities;

public class QuestionAnswer
{
  public int Id { get; set; }
  public LearningPlanQuestion LearnPlanQuestion { get; set; }
  public NotionDbRecord NotionDbRecord { get; set; }
  public bool IsCorrect { get; set; }
}