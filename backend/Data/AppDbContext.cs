using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    /// <summary>
    /// Контекст базы данных, содержащий таблицу с данными сенсоров.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<SensorData> SensorData { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Можно добавить Fluent API для настройки сущностей
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SensorId).IsRequired();
                entity.Property(e => e.Value).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
            });
        }
    }
}
