using ViewCounter.Domain.DTO;
using ViewCounter.Domain.Entities;
using ViewCounter.Domain.ValueObjects;

namespace ViewCounter.Application.Abstractions.Repositories
{
    public interface IViewEventRepository
    {
        Task AddAsync(ViewEvent viewEvent, CancellationToken ct = default);
        Task<bool> ExistsAsync(ViewDeduplicationKey key, DateTime sinceUtc, CancellationToken ct = default);
        Task<int> CountAsync(string entityType, string entityId, CancellationToken ct = default);
        Task<int> CountUniqueAsync(string entityType, string entityId, CancellationToken ct = default);
        Task<List<ViewEvent>> GetRecentAsync(string entityType, int count, CancellationToken ct = default);
        Task<List<PopularEntityDto>> GetPopularAsync(string entityType, int count, DateTime window,  CancellationToken ct = default);
        Task<List<ViewEvent>> GetAll();
    }
}
