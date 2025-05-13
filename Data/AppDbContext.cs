using CakeStore.Models;
using CakeStore.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace CakeStore.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Cake> Cakes => Set<Cake>();
    public DbSet<CakeProperty> CakeProperties => Set<CakeProperty>();
    public DbSet<UserCake> UserCakes => Set<UserCake>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<UserProfile>()
            .HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<UserProfile>(p => p.UserId);

        modelBuilder.Entity<CakeProperty>()
            .HasOne(p => p.Cake)
            .WithMany(c => c.Properties)
            .HasForeignKey(p => p.CakeId);

        modelBuilder.Entity<UserCake>()
            .HasOne(uc => uc.User)
            .WithMany()
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserCake>()
            .HasOne(uc => uc.Cake)
            .WithMany()
            .HasForeignKey(uc => uc.CakeId);

        modelBuilder.Entity<CakeReview>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<CakeReview>()
            .HasOne(r => r.Cake)
            .WithMany()
            .HasForeignKey(r => r.CakeId);
    }

    public DbSet<CakeReview> CakeReviews => Set<CakeReview>();
}

