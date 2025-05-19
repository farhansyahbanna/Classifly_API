using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Classifly_API.Data
{
    public class ClassiflyDbContext : DbContext
    {
        public ClassiflyDbContext(DbContextOptions<ClassiflyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<BorrowRequest> BorrowRequests { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Item>().ToTable("items");
            modelBuilder.Entity<BorrowRequest>().ToTable("borrow_requests");
            modelBuilder.Entity<DamageReport>().ToTable("damage_reports");
            modelBuilder.Entity<Notification>().ToTable("notifications");

            // Configure relationships and constraints
            modelBuilder.Entity<BorrowRequest>()
                .HasOne(br => br.User)
                .WithMany(u => u.BorrowRequests)
                .HasForeignKey(br => br.UserId);

            modelBuilder.Entity<BorrowRequest>()
                .HasOne(br => br.Item)
                .WithMany(i => i.BorrowRequests)
                .HasForeignKey(br => br.ItemId);

            modelBuilder.Entity<DamageReport>()
                .HasOne(dr => dr.User)
                .WithMany(u => u.DamageReports)
                .HasForeignKey(dr => dr.UserId);

            modelBuilder.Entity<DamageReport>()
                .HasOne(dr => dr.Item)
                .WithMany(i => i.DamageReports)
                .HasForeignKey(dr => dr.ItemId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);
        }
    }
}
