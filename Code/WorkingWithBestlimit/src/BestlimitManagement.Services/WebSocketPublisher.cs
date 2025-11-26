using BaseLimitManagement.Contracts;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace BestlimitManagement.Services;

public class WebSocketPublisher : IMessagePublisher
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

    public string? GetActiveInsCode() => _activeInsCode;

    public async Task PushMessage(IEnumerable<BestLimit> data)
    {
        if (_currentClient != null && _currentClient.State == WebSocketState.Open)
        {
            var json = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _currentClient.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
