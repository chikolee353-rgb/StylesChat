using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public sealed class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    public DbSet<ChatUser> Users => Set<ChatUser>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatUser>()
            .HasIndex(user => user.NormalizedUsername)
            .IsUnique();
        modelBuilder.Entity<Message>()
            .HasKey(message => message.Id);
    }
}
