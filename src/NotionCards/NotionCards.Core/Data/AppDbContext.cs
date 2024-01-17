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
      .UseSqlServer(connectionString: "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=NotionCards;TrustServerCertificate=true");
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<NotionDbRecord>().HasKey(x => x.Id);
    modelBuilder.Entity<NotionDbRecord>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<NotionDbRecord>().HasIndex(x => x.NotionId).IsUnique(true);

    modelBuilder.Entity<Card>().HasKey(x => x.Id);
    modelBuilder.Entity<Card>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<Card>().Property(x => x.AddedTime).HasDefaultValueSql("GETUTCDATE()");
    modelBuilder.Entity<Card>().HasIndex(x => x.AddedTime).IsDescending(true);

    modelBuilder.Entity<Set>().HasKey(x => x.Id);
    modelBuilder.Entity<Set>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<Set>()
      .HasMany(x => x.Cards)
      .WithOne(x => x.Set)
      .IsRequired(false);

    modelBuilder.Entity<NotionDbImportHistory>().HasKey(x => x.Id);
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.LastOperation).HasDefaultValueSql("GETUTCDATE()");
    modelBuilder.Entity<NotionDbImportHistory>().Property(x => x.LastOperation).ValueGeneratedOnAdd();
    modelBuilder.Entity<NotionDbImportHistory>().HasIndex(x => x.NotionDbId).IsUnique(true);
  }
}