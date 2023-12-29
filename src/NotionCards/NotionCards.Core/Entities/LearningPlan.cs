namespace NotionCards.Core.Entities;

public class LearningPlan
{
  public int Id { get; set; }
  public int[] Users { get; set; }
  public LearningPlanQuestion[] Questions { get; set; }
  public PlanStatus Status { get; set; }
}