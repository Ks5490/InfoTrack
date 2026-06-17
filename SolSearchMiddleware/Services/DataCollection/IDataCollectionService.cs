using SolSearch.Models;

namespace SolSearch.Services.DataStratergy
{
    public interface IDataCollectionService
    {
        Task<IEnumerable<PotentialClient>> CollectAsync(List<PossibleLocations> possibleLocations, bool enhanced, CancellationToken cancellationToken);
    }
}