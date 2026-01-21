using Microsoft.EntityFrameworkCore;
using ViewCounter.Domain.Entities;

namespace ViewCounter.Infrastructure.Persistence
{
    public class ViewCounterDbContext : DbContext
    {
        public DbSet<ViewEvent> ViewEvents => Set<ViewEvent>();

        public ViewCounterDbContext(DbContextOptions<ViewCounterDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewEvent>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.EntityType).IsRequired();
                entity.Property(x => x.EntityId).IsRequired();
                entity.Property(x => x.IpHash).IsRequired();
                entity.Property(x => x.UserAgentHash).IsRequired();
                entity.Property(x => x.ViewedAt).IsRequired();

                entity.HasIndex(x => new
                {
                    x.EntityType,
                    x.EntityId,
                    x.IpHash,
                    x.UserAgentHash,
                    x.ViewedAt
                });
            });
        }
    }
}
