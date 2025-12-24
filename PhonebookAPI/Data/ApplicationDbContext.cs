using Microsoft.EntityFrameworkCore;
using PhonebookAPI.Models;

namespace PhonebookAPI.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet rappresentano le tabelle
    public DbSet<FirstName> FirstNames { get; set; }
    public DbSet<LastName> LastNames { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PhoneBookEntry> PhoneBookEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Configurazione FirstName
      modelBuilder.Entity<FirstName>(entity =>
      {
        entity.ToTable("FirstNames");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.HasIndex(e => e.Name).IsUnique();
      });

      // Configurazione LastName
      modelBuilder.Entity<LastName>(entity =>
      {
        entity.ToTable("LastNames");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Surname).IsRequired().HasMaxLength(100);
        entity.HasIndex(e => e.Surname).IsUnique();
      });

      // Configurazione User
      modelBuilder.Entity<User>(entity =>
      {
        entity.ToTable("Users");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
        entity.HasIndex(e => e.Email).IsUnique();
        entity.Property(e => e.PasswordHash).IsRequired();
        entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
        entity.Property(e => e.BackgroundImageUrl).HasMaxLength(500);

        // Relazione User -> FirstName
        entity.HasOne(e => e.FirstName)
              .WithMany(f => f.Users)
              .HasForeignKey(e => e.FirstNameId)
              .OnDelete(DeleteBehavior.Restrict);

        // Relazione User -> LastName
        entity.HasOne(e => e.LastName)
              .WithMany(l => l.Users)
              .HasForeignKey(e => e.LastNameId)
              .OnDelete(DeleteBehavior.Restrict);
      });

      // Configurazione PhoneBookEntry
      modelBuilder.Entity<PhoneBookEntry>(entity =>
      {
        entity.ToTable("PhoneBookEntries");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);

        // Relazione PhoneBookEntry -> User (CASCADE DELETE)
        entity.HasOne(e => e.User)
              .WithMany(u => u.PhoneBookEntries)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        // Relazione PhoneBookEntry -> FirstName
        entity.HasOne(e => e.FirstName)
              .WithMany(f => f.PhoneBookEntries)
              .HasForeignKey(e => e.FirstNameId)
              .OnDelete(DeleteBehavior.Restrict);

        // Relazione PhoneBookEntry -> LastName
        entity.HasOne(e => e.LastName)
              .WithMany(l => l.PhoneBookEntries)
              .HasForeignKey(e => e.LastNameId)
              .OnDelete(DeleteBehavior.Restrict);
      });
    }
  }
}
