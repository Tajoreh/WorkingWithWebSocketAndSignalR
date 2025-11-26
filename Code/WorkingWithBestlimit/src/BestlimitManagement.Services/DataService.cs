using BaseLimitManagement.Contracts;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;

namespace BestlimitManagement.Services;

public class DataService(HostConfig hostConfig,IHttpClientFactory clientFactory) : IDataService
{

    public async Task<IEnumerable<Instruments>> FindInstrumentsAsync(string instrumentName)
    {
        using var httpClient = clientFactory.CreateClient();

       var sendUrl = $"{hostConfig.GetInstrumentsPath}/{instrumentName}";

        var response = await httpClient.GetAsync(sendUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var instruments = JsonSerializer.Deserialize<InstrumentSearchResponce>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return instruments.InstrumentSearch ?? Enumerable.Empty<Instruments>();
    }

    public async Task<IEnumerable<BestLimit>> GetBestlimits(string isinCode)
    {
        using var httpClient = clientFactory.CreateClient();

        var sendUrl = $"{hostConfig.GetBestlimitPath}/{isinCode}";

        var response = await httpClient.GetAsync(sendUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var bastlimits = JsonSerializer.Deserialize<BestLimitsResponce>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return bastlimits.BestLimits ?? Enumerable.Empty<BestLimit>();
    }
}
