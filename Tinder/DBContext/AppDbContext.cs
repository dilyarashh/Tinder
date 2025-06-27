using Microsoft.EntityFrameworkCore;
using Tinder.DBContext.DTO.User;
using Tinder.DBContext.Models;

namespace Tinder.DBContext;
public class AppDbcontext(DbContextOptions<AppDbcontext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<BlackToken> BlackTokens { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<Message> Messages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
             .Property(u => u.EducationLevel)
             .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Telegram)
            .IsUnique(); 
    }
}