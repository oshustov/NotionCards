namespace NotionCards.Core.Entities;

public class LearningPlan
{
  public int Id { get; set; }
  public string OwnerUserId { get; set; }
  public string Name { get; set; }

  public LearningPlanQuestion[] Questions { get; set; }
  public PlanStatus Status { get; set; }
}