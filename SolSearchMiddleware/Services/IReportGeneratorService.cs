using SolSearch.Models;

namespace SolSearch.Services
{
    public interface IReportGeneratorService
    {
        Task<IEnumerable<PotentialClient>> GenerateReport(List<PossibleLocations> possibleLocations, bool enhanced, CancellationToken ct);
    }
}