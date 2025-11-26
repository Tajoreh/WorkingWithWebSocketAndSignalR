using BestlimitManagement.Services;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using System.Text.Json;

namespace BestLimtManagement;
public class BestLimitJob(SignalRPublisher wsService, IDataService dataService) : IJob
{
    private readonly SignalRPublisher _publisher = wsService;
    private readonly IDataService _dataService = dataService;

    public async Task Execute(IJobExecutionContext context)
    {
        var insCode = _publisher.GetActiveInsCode();
        if (string.IsNullOrEmpty(insCode)) return;

        var data = await _dataService.GetBestlimits(insCode);

        await _publisher.PushMessage(data);
    }
}