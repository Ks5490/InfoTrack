using SolSearch.Services.ListingWebsites;

namespace SolSearch.Factories
{
    public interface IConveyancingListingSitesFactory
    {
        public IConveyanceListingScrapperService GenSiteScrapper();
    }
}