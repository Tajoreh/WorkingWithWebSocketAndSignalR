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
