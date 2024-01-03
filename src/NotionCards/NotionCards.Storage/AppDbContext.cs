using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Entities;

namespace NotionCards.Storage;

public class AppDbContext : DbContext
{
  public DbSet<LearningPlan> LearningPlans { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlServer(connectionString: "");
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<LearningPlan>().HasKey(x => x.Id);
    modelBuilder.Entity<LearningPlan>().Property(x => x.Id).UseIdentityColumn();

    modelBuilder.Entity<LearningPlan>()
      .HasMany(x => x.Questions)
      .WithOne(x => x.LearningPlan)
      .HasForeignKey(x => x.LearningPlanId);

    modelBuilder.Entity<LearningPlanQuestion>().HasKey(x => x.Id);
    modelBuilder.Entity<LearningPlanQuestion>().Property(x => x.Id).UseIdentityColumn();

    modelBuilder.Entity<LearningPlanQuestion>()
      .HasMany(x => x.Answers)
      .WithOne(x => x.LearnPlanQuestion);

    modelBuilder.Entity<LearningPlanQuestion>()
      .HasOne(x => x.Progress)
      .WithOne(x => x.Question);

    modelBuilder.Entity<QuestionAnswer>().HasKey(x => x.Id);
    modelBuilder.Entity<QuestionAnswer>().Property(x => x.Id).UseIdentityColumn();

    modelBuilder.Entity<QuestionAnswer>()
      .HasOne(x => x.NotionDbEntry);


    modelBuilder.Entity<Progress>().HasKey(x => new {QuestionId = x.Question.Id, AnswerId = x.Answer.Id});
    modelBuilder.Entity<Progress>()
      .HasOne(x => x.Answer);
  }
}