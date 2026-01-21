using ViewCounter.Domain.Entities;

namespace ViewCounter.Domain.Interfaces
{
    public interface IViewDeduplicationPolicy
    {
        Task<bool> CanCountAsync(ViewEvent viewEvent);
    }
}
