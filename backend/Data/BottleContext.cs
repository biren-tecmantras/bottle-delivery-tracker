using BottleDeliveryTracker.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace BottleDeliveryTracker.Backend.Data
{
    public class BottleContext : DbContext
    {
        public BottleContext(DbContextOptions<BottleContext> options)
            : base(options)
        {
        }

        public DbSet<DeliveryRecord> DeliveryRecords => Set<DeliveryRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeliveryRecord>()
                .HasIndex(r => r.Date);

            modelBuilder.Entity<DeliveryRecord>()
                .Property(r => r.Count)
                .HasDefaultValue(0);
        }
    }
}
