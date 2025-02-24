using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace API.Context
{
    public class AppDBContext : DbContext
    {
            public AppDBContext(DbContextOptions<AppDBContext> options)
                : base(options)
            {
            }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<UserDevice> UserDevice { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }


        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Common &&
                            (e.State == EntityState.Added
                            || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (Common)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    if (string.IsNullOrEmpty(entity.Id))
                    {
                        entity.Id = Guid.NewGuid().ToString();
                    }
                    entity.CreatedAt = DateTime.UtcNow.AddHours(2);
                }

            }
        }


        public DbSet<API.Models.ActivityLog> ActivityLogs { get; set; }
    }

}
