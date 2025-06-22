using GymTracker.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.UserService.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // הגדרות לטבלת Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);  // Id זה Primary Key
            entity.Property(e => e.Email)
                  .IsRequired()        // Email חובה
                  .HasMaxLength(256);  // מקסימום 256 תווים

            entity.HasIndex(e => e.Email)
                  .IsUnique();        // Email חייב להיות יוניק
        });
    }
}