using BestlimitManagement.Services;
using Quartz;

namespace BestLimtManagement;

public class WebsocketJob(WebSocketPublisher wsService, IDataService dataService) : IJob
{
    private readonly WebSocketPublisher _publisher = wsService;
    private readonly IDataService _dataService = dataService;

    public async Task Execute(IJobExecutionContext context)
    {
        var insCode = _publisher.GetActiveInsCode();
        if (string.IsNullOrEmpty(insCode)) return;

        var data = await _dataService.GetBestlimits(insCode);

        await _publisher.PushMessage(data);
    }
}