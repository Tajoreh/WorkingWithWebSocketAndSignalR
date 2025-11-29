using BaseLimitManagement.Contracts;
using System.Net.WebSockets;

namespace BestlimitManagement.Services;


using Microsoft.AspNetCore.SignalR;
public class SignalRPublisher 
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