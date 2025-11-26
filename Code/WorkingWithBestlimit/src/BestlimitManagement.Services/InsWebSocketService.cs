using BaseLimitManagement.Contracts;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace BestlimitManagement.Services;

public class InsWebSocketService
{
    private WebSocket? _currentClient;
    private string? _activeInsCode;

    public async Task HandleClient(WebSocket socket, string insCode)
    {
        if (_currentClient != null && _currentClient.State == WebSocketState.Open)
        {
            await _currentClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "New client connected", CancellationToken.None);
        }

        _currentClient = socket;
        _activeInsCode = insCode;

        var buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            var res = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (res.MessageType == WebSocketMessageType.Close) break;
        }

        _currentClient = null;
        _activeInsCode = null;
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
    }

    public async Task PushUpdateAsync(IEnumerable<BestLimit> data)
    {
        if (_currentClient != null && _currentClient.State == WebSocketState.Open)
        {
            var json = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _currentClient.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public string? GetActiveInsCode() => _activeInsCode;
}

public class BestLimitHub : Hub
{
    private readonly BestLimitHubService _hubService;

    public BestLimitHub(BestLimitHubService hubService)
    {
        _hubService = hubService;
    }

    public async Task Subscribe(string insCode)
    {
        _hubService.SetInsCode(insCode);
        await Clients.Caller.SendAsync("Subscribed", insCode);
    }
}
public class BestLimitHubService
{
    private string? _activeInsCode;

    public void SetInsCode(string insCode) => _activeInsCode = insCode;
    public string? GetActiveInsCode() => _activeInsCode;

    private readonly IHubContext<BestLimitHub> _hubContext;

    public BestLimitHubService(IHubContext<BestLimitHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PushUpdateAsync(IEnumerable<BestLimit> data)
    {
        if (_activeInsCode != null)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveBestLimit", data);
        }
    }
}