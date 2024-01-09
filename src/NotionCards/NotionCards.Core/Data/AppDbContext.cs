using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Entities;

namespace NotionCards.Storage;

public class AppDbContext : DbContext
{
  public DbSet<LearningPlan> LearningPlans { get; set; }
  public DbSet<NotionDbRecord> NotionDbRecords { get; set; }
  public DbSet<NotionDbImportHistory> ImportHistories { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder
      .UseSqlServer(connectionString: "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=NotionCards;TrustServerCertificate=true");
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>().HasKey(x => x.Id);
    modelBuilder.Entity<User>()
      .Property(x => x.Id)
      .ValueGeneratedNever();

    modelBuilder.Entity<NotionDbRecord>().HasKey(x => x.Id);
    modelBuilder.Entity<NotionDbRecord>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<NotionDbRecord>().HasIndex(x => x.NotionId).IsUnique(true);

    modelBuilder.Entity<QuestionAnswer>().HasKey(x => x.Id);
    modelBuilder.Entity<QuestionAnswer>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<QuestionAnswer>().HasOne(x => x.NotionDbRecord);

    modelBuilder.Entity<Progress>().HasKey(x => new { x.AnswerId, x.QuestionId });
    modelBuilder.Entity<Progress>().HasOne(x => x.Answer);

    modelBuilder.Entity<LearningPlanQuestion>().HasKey(x => x.Id);
    modelBuilder.Entity<LearningPlanQuestion>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<LearningPlanQuestion>()
      .HasMany(x => x.Answers)
      .WithOne(x => x.LearnPlanQuestion)
      .IsRequired(false);
    modelBuilder.Entity<LearningPlanQuestion>()
      .HasOne(x => x.Progress)
      .WithOne(x => x.Question)
      .IsRequired(true);

    modelBuilder.Entity<LearningPlan>().HasKey(x => x.Id);
    modelBuilder.Entity<LearningPlan>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<LearningPlan>()
      .HasMany(x => x.Questions)
      .WithOne(x => x.LearningPlan)
      .IsRequired(false);

    modelBuilder.Entity<NotionDbImportHistory>().HasKey(x => x.Id);
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.LastOperation).HasDefaultValueSql("GETUTCDATE()");
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.LastOperation).ValueGeneratedOnAdd();
    modelBuilder.Entity<NotionDbImportHistory>().HasIndex(x => x.NotionDbId).IsUnique(true);
  }
}