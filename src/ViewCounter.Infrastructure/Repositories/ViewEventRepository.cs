using Microsoft.EntityFrameworkCore;
using System.Linq;
using ViewCounter.Application.Abstractions.Repositories;
using ViewCounter.Domain.DTO;
using ViewCounter.Domain.Entities;
using ViewCounter.Domain.ValueObjects;
using ViewCounter.Infrastructure.Persistence;

namespace ViewCounter.Infrastructure.Repositories
{
    public class ViewEventRepository : IViewEventRepository
    {
        private readonly ViewCounterDbContext _db;

        public ViewEventRepository(ViewCounterDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(ViewEvent viewEvent, CancellationToken ct = default)
        {
            _db.ViewEvents.Add(viewEvent);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(ViewDeduplicationKey key, DateTime sinceUtc, CancellationToken ct = default)
        {
            return await _db.ViewEvents.AnyAsync(v =>
                v.EntityType == key.EntityType &&
                v.EntityId == key.EntityId &&
                v.IpHash == key.IpHash &&
                v.UserAgentHash == key.UserAgentHash &&
                v.ViewedAt >= sinceUtc,
                ct);
        }
        public async Task<int> CountAsync(string entityType, string entityId, CancellationToken ct = default)
        {
            return await _db.ViewEvents
                .CountAsync(v => v.EntityType == entityType && v.EntityId == entityId, ct);
        }

        public async Task<int> CountUniqueAsync(string entityType, string entityId, CancellationToken ct = default)
        {
            return await _db.ViewEvents
                .Where(v => v.EntityType == entityType && v.EntityId == entityId)
                .Select(v => new { v.IpHash, v.UserAgentHash })
                .Distinct()
                .CountAsync(ct);
        }

        public async Task<List<ViewEvent>> GetRecentAsync(
            string entityType, int count, CancellationToken ct = default)
        {
            return await _db.ViewEvents
                .Where(v => v.EntityType == entityType)
                .Distinct()
                .OrderByDescending(v => v.ViewedAt)
                .Take(count)
                .ToListAsync(ct);
        }

        public async Task<List<PopularEntityDto>> GetPopularAsync(
            string entityType, int count,
            DateTime window, CancellationToken ct = default)
        {
            return await _db.ViewEvents
                .Where(v =>
                    v.EntityType == entityType &&
                    v.ViewedAt >= window)
                .GroupBy(v => v.EntityId)
                .Select(g => new PopularEntityDto
                {
                    EntityId = g.Key,
                    Views = g.Count(),
                    LastViewedAt = g.Max(v => v.ViewedAt)
                })
                .OrderByDescending(x => x.Views)
                .ThenByDescending(x => x.LastViewedAt)
                .Take(count)
                .ToListAsync(ct);
        }

        public async Task<List<ViewEvent>> GetAll()
        {
            return await _db.ViewEvents.ToListAsync();
        }
    }
}
