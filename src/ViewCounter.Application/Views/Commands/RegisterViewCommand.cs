namespace ViewCounter.Application.Views.Commands
{
    public record RegisterViewCommand(
        string EntityType,
        string EntityId,
        string? UserId,
        string IpAddress,
        string UserAgent
        );
}
