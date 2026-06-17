using SolSearch.Models;

namespace SolSearch.Services.ListingWebsites
{
    public interface IConveyanceListingScrapperService
    {
        public Task<IEnumerable<PotentialClient>> GetListingsAsync(List<PossibleLocations> possibleLocations, bool enhanced, CancellationToken cancellationToken);
    }
}
