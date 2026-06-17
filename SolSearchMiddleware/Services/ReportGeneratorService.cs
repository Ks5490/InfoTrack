using SolSearch.Models;
using SolSearch.Services.API;
using SolSearch.Services.DataStratergy;

namespace SolSearch.Services
{
    public class ReportGeneratorService(IDataCollectionService dataCollector) : IReportGeneratorService
    {
        public async Task<IEnumerable<PotentialClient>> GenerateReport(List<PossibleLocations> possibleLocations, bool enhanced, CancellationToken ct)
        {
            // Can use strategy pattern for different use cases e.g. api/ read from excell ect. explore keyed services
            var report = await dataCollector.CollectAsync(possibleLocations, enhanced, ct);
            return report;
        }
    }
}  
