using ViewCounter.Application.Abstractions.Repositories;
using ViewCounter.Application.Abstractions.Services;
using ViewCounter.Application.Views.Commands;
using ViewCounter.Domain.Entities;
using ViewCounter.Domain.Interfaces;

namespace ViewCounter.Application.Views.Handlers
{
    public class RegisterViewHandler
    {
        private readonly IViewEventRepository _repository;
        private readonly IHashService _hashService;
        private readonly IViewDeduplicationPolicy _deduplicationPolicy;
        private readonly IViewRateLimiterService _rateLimiter;

        public RegisterViewHandler(
            IViewEventRepository repository,
            IHashService hashService,
            IViewDeduplicationPolicy deduplicationPolicy,
            IViewRateLimiterService rateLimiter)
        {
            _repository = repository;
            _hashService = hashService;
            _deduplicationPolicy = deduplicationPolicy;
            _rateLimiter = rateLimiter;
        }

        public async Task<bool> HandleAsync(
        RegisterViewCommand command,
        CancellationToken ct = default)
        {
            var ipHash = _hashService.Hash(command.IpAddress);
            var uaHash = _hashService.Hash(command.UserAgent);

            var viewEvent = ViewEvent.Create(
                command.EntityType,
                command.EntityId,
                command.UserId,
                ipHash,
                uaHash,
                DateTime.UtcNow
            );

            if (_rateLimiter.IsBot(ipHash))
                return false;

            var canCount = await _deduplicationPolicy.CanCountAsync(viewEvent);
            if (!canCount)
                return false;

            await _repository.AddAsync(viewEvent, ct);
            return true;
        }
    }
}
