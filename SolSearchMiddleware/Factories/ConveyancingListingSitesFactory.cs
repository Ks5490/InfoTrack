using Microsoft.Extensions.Options;
using SolSearch.Models.SettingTypes;
using SolSearch.Services.ListingWebsites;

namespace SolSearch.Factories
{
    public class ConveyancingListingSitesFactory(IServiceProvider serviceProvider) : IConveyancingListingSitesFactory
    {
        public IConveyanceListingScrapperService GenSiteScrapper()
        {
            // open for extension if other conveyancing listing sites are added in the future
            return serviceProvider.GetRequiredService<SolicitorsListingService>();
        }
    }
}
