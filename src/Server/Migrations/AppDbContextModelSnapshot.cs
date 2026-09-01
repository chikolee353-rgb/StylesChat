using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Server.Migrations
{
    [DbContext(typeof(Server.Data.AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("Server.Models.User", b =>
            {
                b.Property<string>("Id").HasColumnType("nvarchar(450)");
                b.Property<string>("ConnectionId").HasColumnType("nvarchar(max)");
                b.Property<string>("DisplayName").IsRequired().HasColumnType("nvarchar(max)");
                b.Property<string>("PasswordHash").IsRequired().HasColumnType("nvarchar(max)");
                b.Property<string>("Username").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
                b.HasKey("Id");
                b.HasIndex("Username").IsUnique();
                b.ToTable("Users");
            });

            modelBuilder.Entity("Server.Models.Group", b =>
            {
                b.Property<string>("Id").HasColumnType("nvarchar(450)");
                b.Property<string>("Name").IsRequired().HasColumnType("nvarchar(max)");
                b.HasKey("Id");
                b.ToTable("Groups");
            });

            modelBuilder.Entity("Server.Models.GroupMember", b =>
            {
                b.Property<string>("Id").HasColumnType("nvarchar(450)");
                b.Property<string>("GroupId").IsRequired().HasColumnType("nvarchar(450)");
                b.Property<string>("UserId").IsRequired().HasColumnType("nvarchar(450)");
                b.HasKey("Id");
                b.HasIndex("GroupId");
                b.ToTable("GroupMembers");
            });

            modelBuilder.Entity("Server.Models.Message", b =>
            {
                b.Property<string>("Id").HasColumnType("nvarchar(450)");
                b.Property<string>("Attachments").HasColumnType("nvarchar(max)");
                b.Property<string>("RecipientIds").HasColumnType("nvarchar(max)");
                b.Property<DateTime>("Timestamp").HasColumnType("datetime2");
                b.Property<string>("Text").HasColumnType("nvarchar(max)");
                b.Property<string>("SenderId").IsRequired().HasColumnType("nvarchar(450)");
                b.HasKey("Id");
                b.HasIndex("SenderId");
                b.ToTable("Messages");
            });
        }
    }
}
