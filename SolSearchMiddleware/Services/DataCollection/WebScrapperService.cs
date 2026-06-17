using Microsoft.Extensions.Options;
using SolSearch.Factories;
using SolSearch.Models;
using SolSearch.Models.SettingTypes;
using SolSearch.Services.API;
using SolSearch.Services.ListingWebsites;

namespace SolSearch.Services.DataStratergy
{
    public class WebScrapperService(IOptions<PotentialClientUrls> options, IConveyancingListingSitesFactory conveyancingListingSitesFactory, 
        IApiService apiService) : IDataCollectionService
    {
        public async Task<IEnumerable<PotentialClient>> CollectAsync(List<PossibleLocations> possibleLocations, bool enhanced, CancellationToken cancellationToken)
        {
            IConveyanceListingScrapperService siteScrapper = conveyancingListingSitesFactory.GenSiteScrapper();
            return await siteScrapper.GetListingsAsync(possibleLocations, enhanced, cancellationToken);
        }
    }
}
