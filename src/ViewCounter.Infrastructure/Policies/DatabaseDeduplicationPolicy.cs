using ViewCounter.Application.Abstractions.Repositories;
using ViewCounter.Domain.Entities;
using ViewCounter.Domain.Interfaces;
using ViewCounter.Domain.ValueObjects;

namespace ViewCounter.Infrastructure.Policies
{
    public class DatabaseDeduplicationPolicy : IViewDeduplicationPolicy
    {
        private static readonly TimeSpan Window = TimeSpan.FromHours(24);

        private readonly IViewEventRepository _repository;

        public DatabaseDeduplicationPolicy(IViewEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CanCountAsync(ViewEvent viewEvent)
        {
            var key = new ViewDeduplicationKey(
                viewEvent.EntityType,
                viewEvent.EntityId,
                viewEvent.IpHash,
                viewEvent.UserAgentHash);

            var since = viewEvent.ViewedAt.Subtract(Window);

            return !await _repository.ExistsAsync(key, since);
        }
    }
}
