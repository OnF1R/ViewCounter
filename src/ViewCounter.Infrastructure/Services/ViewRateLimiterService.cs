using Microsoft.Extensions.Caching.Memory;
using ViewCounter.Application.Abstractions.Services;

namespace ViewCounter.Infrastructure.Services
{
    public class ViewRateLimiterService : IViewRateLimiterService
    {
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan Window = TimeSpan.FromSeconds(10);
        private const int MaxViews = 5;

        private const string CachePrefix = "view:rate:";

        public ViewRateLimiterService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool IsBot(string hashId)
        {
            var now = DateTime.UtcNow;
            var key = $"{CachePrefix}{hashId}";

            var timestamps = _cache.GetOrCreate(
                key,
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = Window;
                    return new List<DateTime>();
                });

            if (timestamps is null)
                return false;

            timestamps.RemoveAll(t => t < now - Window);
            timestamps.Add(now);

            return timestamps.Count > MaxViews;
        }
    }
}
