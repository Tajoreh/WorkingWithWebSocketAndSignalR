using BaseLimitManagement.Contracts;
using BestlimitManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace BestLimtManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class SymbolsController(IDataService dataservice, ILogger<SymbolsController> logger) : ControllerBase
{

    [HttpGet("Instruments/{instrumentName}")]
    public async Task<IEnumerable<Instruments>> GetInstruments([FromRoute]string instrumentName)
    {
               
        return await dataservice.FindInstrumentsAsync(instrumentName);
    }

    [HttpGet("Bestlimits/{isinCode}")]
    public async Task<IEnumerable<BestLimit>> GetBestlimit([FromRoute]string isinCode)
    {
        return await dataservice.GetBestlimits(isinCode);
    }


}
