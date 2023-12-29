namespace NotionCards.Core.Entities;

public class LearningPlanQuestion
{
  public int Id { get; set; }
  public LearningPlan LearningPlan { get; set; }
  public Progress Progress { get; set; }

  public QuestionAnswer[] Answers { get; set; }
}