using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Entities;

namespace NotionCards.Storage;

public class AppDbContext : DbContext
{
  public DbSet<LearningPlan> LearningPlans { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<LearningPlan>().HasMany(x => x.Questions);

    modelBuilder.Entity<LearningPlanQuestion>().HasMany(x => x.Answers);
    modelBuilder.Entity<LearningPlanQuestion>().HasOne(x => x.LearningPlan);
    modelBuilder.Entity<LearningPlanQuestion>().HasOne(x => x.Progress);

    modelBuilder.Entity<QuestionAnswer>().HasOne(x => x.NotionDbEntry);
    modelBuilder.Entity<QuestionAnswer>().HasOne(x => x.LearnPlanQuestion);

    modelBuilder.Entity<Progress>().HasOne(x => x.Question);
    modelBuilder.Entity<Progress>().HasOne(x => x.Answer);
  }
}