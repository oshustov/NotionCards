namespace NotionCards.Core.Entities;

public class Progress
{
  public int QuestionId { get; set; }
  public int AnswerId { get; set; }

  public LearningPlanQuestion Question { get; set; }
  public QuestionAnswer Answer { get; set; }
}