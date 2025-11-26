using BaseLimitManagement.Contracts;

namespace BestlimitManagement.Services;

public interface IMessagePublisher
{
    public Task PushMessage(IEnumerable<BestLimit> data);
    string? GetActiveInsCode();

}
