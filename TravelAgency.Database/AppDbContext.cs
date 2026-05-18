using Microsoft.EntityFrameworkCore;
using TravelAgency.Database.Models;

namespace TravelAgency.Database
{
    public class AppDbContext : DbContext
    {
        // Конструктор без параметров (нужен для Design-Time)
        public AppDbContext()
        {
        }

        // Конструктор с параметрами (для Dependency Injection)
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Destination).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Destination);
            });
            
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ClientEmail).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
            });
            
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tour)
                    .WithMany(t => t.OrderItems)
                    .HasForeignKey(e => e.TourId);
            });
        }
    }
}