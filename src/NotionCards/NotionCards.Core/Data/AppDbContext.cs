using Microsoft.EntityFrameworkCore;
using NotionCards.Core.Entities;

namespace NotionCards.Storage;

public class AppDbContext : DbContext
{
  public DbSet<Set> Sets { get; set; }
  public DbSet<Card> Cards { get; set; } 

  public DbSet<NotionDbSetup> NotionDbs { get; set; }
  public DbSet<NotionDbPull> NotionDbPulls { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder
      .UseSqlServer(connectionString: "Server=(LocalDb)\\MSSQLLocalDB;Database=LeitnerCards;TrustServerCertificate=true");
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
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

    modelBuilder.Entity<BoxCard>().HasKey(x => x.Id);
    modelBuilder.Entity<BoxCard>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<BoxCard>().HasIndex(x => x.LeitnerBoxId);

    modelBuilder.Entity<Box>().HasKey(x => x.Id);
    modelBuilder.Entity<Box>().Property(x => x.Id).UseIdentityColumn();
    modelBuilder.Entity<Box>().HasIndex(x => x.SetId);

    modelBuilder.Entity<UserSetState>().HasKey(x => new { x.UserId, x.SetId });
    modelBuilder.Entity<UserSetState>().HasIndex(x => new { x.UserId, x.SetId });

    modelBuilder.Entity<NotionDbSetup>().HasKey(x => x.NotionDbId);
    modelBuilder.Entity<NotionDbSetup>().Property(x => x.PullingInterval).IsRequired(false);

    modelBuilder.Entity<NotionDbPull>().HasKey(x => x.NotionDbId);
    modelBuilder.Entity<NotionDbPull>().Property(x => x.LastRecordDateTime).IsRequired();
  }
}