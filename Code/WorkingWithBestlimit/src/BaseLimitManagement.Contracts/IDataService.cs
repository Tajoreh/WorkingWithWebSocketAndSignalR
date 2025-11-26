using BaseLimitManagement.Contracts;

namespace BestlimitManagement.Services;

public interface IDataService
{
    public Task<IEnumerable<Instruments>> FindInstrumentsAsync(string instrumentName);
    public Task<IEnumerable<BestLimit>> GetBestlimits(string isinCode);
}
