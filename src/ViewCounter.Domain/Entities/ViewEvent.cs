namespace ViewCounter.Domain.Entities
{
    public class ViewEvent
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string EntityType { get; init; } = null!;
        public string EntityId { get; init; } = null!;
        public string? UserId { get; init; }
        public string IpHash { get; init; } = null!;
        public string UserAgentHash { get; init; } = null!;
        public DateTime ViewedAt { get; init; }

        private ViewEvent() { }

        public static ViewEvent Create(string entityType, string entityId, string? userId, string ipHash,
            string userAgentHash, DateTime viewedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException("EntityType is required");

            if (string.IsNullOrWhiteSpace(entityId))
                throw new ArgumentException("EntityId is required");

            return new ViewEvent
            {
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                IpHash = ipHash,
                UserAgentHash = userAgentHash,
                ViewedAt = viewedAtUtc
            };
        }
    }
}
