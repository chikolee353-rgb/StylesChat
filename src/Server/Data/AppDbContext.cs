using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

// AppDbContext mirrors ApplicationDbContext and exposes DbSets used by the app.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Message>().HasKey(m => m.Id);
        modelBuilder.Entity<Group>().HasKey(g => g.Id);
        modelBuilder.Entity<GroupMember>().HasKey(gm => gm.Id);
    }
}
