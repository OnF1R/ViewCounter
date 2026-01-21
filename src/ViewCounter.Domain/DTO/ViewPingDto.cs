namespace ViewCounter.Domain.DTO
{
    public record ViewPingDto
    (
        string EntityType,
        string EntityId,
        string? UserId
    );
}
