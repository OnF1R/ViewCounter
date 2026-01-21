namespace ViewCounter.Application.Abstractions.Services
{
    public interface IViewRateLimiterService
    {
        /// <summary>
        /// Регистрирует попытку просмотра и определяет,
        /// превышен ли лимит (бот).
        /// </summary>
        bool IsBot(string hashId);
    }
}
