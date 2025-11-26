using BaseLimitManagement.Contracts;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;

namespace BestlimitManagement.Services;


using Microsoft.AspNetCore.SignalR;

public class BestLimitHub : Hub
{
    private readonly SignalRPublisher _publisher;

    public BestLimitHub(SignalRPublisher publisher)
    {
        _publisher = publisher;
    }

    // وقتی کلاینت Subscribe می‌کند
    public async Task Subscribe(string insCode)
    {
        _publisher.SetActiveInsCode(insCode); // سرویس را بروزرسانی می‌کنیم
        await Clients.Caller.SendAsync("Subscribed", insCode);
    }
}
public class SignalRPublisher : IMessagePublisher
{
    private readonly IHubContext<BestLimitHub> _hubContext;
    private string? _activeInsCode;

    public SignalRPublisher(IHubContext<BestLimitHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public void SetActiveInsCode(string insCode)
    {
        _activeInsCode = insCode;
    }

    public string? GetActiveInsCode() => _activeInsCode;

    public async Task PushMessage(IEnumerable<BestLimit> data)
    {
        if (!string.IsNullOrEmpty(_activeInsCode))
        {
            await _hubContext.Clients.All.SendAsync("ReceiveBestLimit", data);
        }
    }
}