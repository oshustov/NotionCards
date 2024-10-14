using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Entities;

namespace NotionCards.Storage;

public class AppDbContext : DbContext
{
  public DbSet<Set> Sets { get; set; }
  public DbSet<Card> Cards { get; set; } 
  public DbSet<NotionDbRecord> NotionDbRecords { get; set; }
  public DbSet<NotionDbImportHistory> ImportHistories { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder
      .UseSqlServer(connectionString: "Data SourceKind=(LocalDb)\\MSSQLLocalDB;Initial Catalog=LeitnerCards;TrustServerCertificate=true");
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<NotionDbRecord>().HasKey(x => x.Id);
    modelBuilder.Entity<NotionDbRecord>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<NotionDbRecord>().HasIndex(x => x.NotionId).IsUnique(true);
    modelBuilder.Entity<NotionDbRecord>().Property(x => x.NotionDbId).IsRequired();
    modelBuilder.Entity<NotionDbRecord>().HasIndex(x => x.DateAdded).IsDescending();

    modelBuilder.Entity<Card>().HasKey(x => x.Id);
    modelBuilder.Entity<Card>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<Card>().Property(x => x.AddedTime).HasDefaultValueSql("GETUTCDATE()");
    modelBuilder.Entity<Card>().HasIndex(x => x.AddedTime).IsDescending(true);

    modelBuilder.Entity<Set>().HasKey(x => x.Id);
    modelBuilder.Entity<Set>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<Set>().Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
    modelBuilder.Entity<Set>()
      .HasMany(x => x.Boxes)
      .WithOne(x => x.Set)
      .IsRequired();

    modelBuilder.Entity<LeitnerBoxCard>().HasKey(x => x.Id);
    modelBuilder.Entity<LeitnerBoxCard>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<LeitnerBoxCard>().HasIndex(x => x.LeitnerBoxId);

    modelBuilder.Entity<LeitnerBox>().HasKey(x => x.Id);
    modelBuilder.Entity<LeitnerBox>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<LeitnerBox>().HasIndex(x => x.SetId);

    modelBuilder.Entity<UserSetState>().HasKey(x => new { x.UserId, x.SetId });
    modelBuilder.Entity<UserSetState>().HasIndex(x => new { x.UserId, x.SetId });

    modelBuilder.Entity<NotionDbImportHistory>().HasKey(x => x.Id);
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.LastOperation).HasDefaultValueSql("GETUTCDATE()");
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.LastOperation).ValueGeneratedOnAdd();
    modelBuilder.Entity<NotionDbImportHistory>().HasIndex(x => x.NotionDbId).IsUnique(true);
  }
}