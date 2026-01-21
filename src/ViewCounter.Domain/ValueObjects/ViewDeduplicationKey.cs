namespace ViewCounter.Domain.ValueObjects
{
    public readonly record struct ViewDeduplicationKey(
        string EntityType, string EntityId, 
        string IpHash,string UserAgentHash);
}
