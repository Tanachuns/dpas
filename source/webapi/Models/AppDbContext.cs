using Microsoft.EntityFrameworkCore;
using webapi.Models.Entities;

namespace server.Models
{
    public class AppDbContext : DbContext
    {
        IConfiguration configuration;
        public AppDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public DbSet<RegionEntity> Regions { get; set; }
        public DbSet<AlertSettingEntity> AlertSettings { get; set; }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                if (entity.State == EntityState.Modified)
                {
                    ((BaseEntity)entity.Entity).UpdatedAt = now;
                }
            }


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(configuration["ConnectionStrings:DefaultConnection"]);
    }
}