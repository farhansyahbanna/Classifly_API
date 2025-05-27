using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Classifly_API.Data
{
    public class ClassiflyDbContext : DbContext
    {
        public ClassiflyDbContext(DbContextOptions<ClassiflyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BorrowRequest> BorrowRequests { get; set; }
        public DbSet<BorrowItem> BorrowItems { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurasi nama tabel
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Item>().ToTable("items");
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<BorrowRequest>().ToTable("borrow_requests");
            modelBuilder.Entity<BorrowItem>().ToTable("borrow_items");
            modelBuilder.Entity<DamageReport>().ToTable("damage_reports");
            modelBuilder.Entity<Notification>().ToTable("notifications");

            // Konfigurasi kunci utama untuk BorrowItem (junction table)
            modelBuilder.Entity<BorrowItem>()
                .HasKey(bi => new { bi.BorrowRequestId, bi.ItemId });

            // Relasi antara BorrowRequest dan BorrowItem
            modelBuilder.Entity<BorrowItem>()
                .HasOne(bi => bi.BorrowRequest)
                .WithMany(br => br.BorrowItems)
                .HasForeignKey(bi => bi.BorrowRequestId);

            // Relasi antara Item dan BorrowItem
            modelBuilder.Entity<BorrowItem>()
                .HasOne(bi => bi.Item)
                .WithMany(i => i.BorrowItems)
                .HasForeignKey(bi => bi.ItemId);

            // Relasi antara Item dan Category
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId);

            // Relasi antara BorrowRequest dan User
            modelBuilder.Entity<BorrowRequest>()
                .HasOne(br => br.User)
                .WithMany(u => u.BorrowRequests)
                .HasForeignKey(br => br.UserId);

            // Relasi antara DamageReport dan User
            modelBuilder.Entity<DamageReport>()
                .HasOne(dr => dr.User)
                .WithMany(u => u.DamageReports)
                .HasForeignKey(dr => dr.UserId);

            // Relasi antara DamageReport dan Item
            modelBuilder.Entity<DamageReport>()
                .HasOne(dr => dr.Item)
                .WithMany(i => i.DamageReports)
                .HasForeignKey(dr => dr.ItemId);

            // Relasi antara Notification dan User
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            // Konfigurasi default value untuk CreatedAt
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Item>()
                .Property(i => i.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<BorrowRequest>()
                .Property(br => br.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<DamageReport>()
                .Property(dr => dr.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}