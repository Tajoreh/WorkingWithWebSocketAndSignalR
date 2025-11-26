using BestlimitManagement.Services;
using Quartz;
using System.Text.Json;

namespace BestLimtManagement;
public class BestLimitJob : IJob
{
    private readonly InsWebSocketService _wsService;
    private readonly IDataService _dataService;

    public BestLimitJob(InsWebSocketService wsService, IDataService dataService)
    {
        _wsService = wsService;
        _dataService = dataService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var insCode = _wsService.GetActiveInsCode();
        if (string.IsNullOrEmpty(insCode)) return;

        var data = await _dataService.GetBestlimits(insCode);
        await _wsService.PushUpdateAsync(data);
    }
}