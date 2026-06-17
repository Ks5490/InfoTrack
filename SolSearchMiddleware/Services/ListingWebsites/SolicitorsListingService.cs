using Microsoft.Extensions.Options;
using SolSearch.Models;
using SolSearch.Models.SettingTypes;
using SolSearch.Services.API;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace SolSearch.Services.ListingWebsites
{
    public class SolicitorsListingService(IApiService apiService, IOptions<PotentialClientUrls> options) : IConveyanceListingScrapperService
    {
        private readonly PotentialClientUrls _urls = options.Value;

        public async Task<IEnumerable<PotentialClient>> GetListingsAsync(List<PossibleLocations> possibleLocations, bool enhanced, CancellationToken cancellationToken)
        {     
                var urls = GenerateUrls(possibleLocations);

                List<Task<IEnumerable<PotentialClient>>> tasks = new List<Task<IEnumerable<PotentialClient>>>();

                foreach (var url in urls)
                {
                    tasks.Add(GetIndividuallDataAsTask(url, enhanced, cancellationToken));
                }

                var results = await Task.WhenAll(tasks);
                return results.SelectMany(x => x).ToList();      
        }

        private List<string> GenerateUrls(List<PossibleLocations> possibleLocations)
        {
            List<string> locationUrls = new List<string>();
            foreach (PossibleLocations possibleLocation in possibleLocations)
            {
                locationUrls.Add(GenerateUrl(possibleLocation.ToString()));
            }
            return locationUrls;
        }

        private string GenerateUrl(string location)
        {
            return String.Format(_urls.Solicitors, location);
        }

        private async Task<IEnumerable<PotentialClient>> GetIndividuallDataAsTask(string url, bool enhanced, CancellationToken ct)
        {
            try
            {
                var websiteData = await apiService.GetAsync(url, ct);
                return await TransformHtmlToData(websiteData, enhanced, ct);
            }
            catch (Exception ex) 
            {
                // log exception for visibility
                return new List<PotentialClient>();
            }
        }


        private async Task<IEnumerable<PotentialClient>> TransformHtmlToData(string rawHtml, bool enhanced, CancellationToken ct)
        {
            var resultsSection = ExtractResultsSection(rawHtml);

            var potentialClients = await TransformResultToPotentialClientList(resultsSection, enhanced, ct);

            return potentialClients;
        }

        private string ExtractResultsSection(string rawHtml)
        {
            // could cache dynamic offset 
            var resultsSectionIndex = rawHtml.IndexOf("result-section", 10000);

            if (resultsSectionIndex == -1) { throw new InvalidOperationException("Could not find a results section - check if HTML structure has changed"); } // custom exception type MVP

            var fromResultsSection = rawHtml.Substring(resultsSectionIndex);

            int resultendIndex = fromResultsSection.LastIndexOf("result-item item-small");
            resultendIndex = resultendIndex == -1
                ? fromResultsSection.LastIndexOf("result-item")
                : resultendIndex;

            if (resultendIndex == -1) { throw new InvalidOperationException("Could not find end of results section - check if HTML structure has changed"); }

            return rawHtml.Substring(resultsSectionIndex, resultendIndex - resultsSectionIndex);
        }

        private async Task<List<PotentialClient>> TransformResultToPotentialClientList(string resultsSection, bool enhanced, CancellationToken ct)
        {
            var potentialClients = new List<PotentialClient>();
            string[] largeResults = resultsSection.Split("result-item").Skip(1).ToArray();

            foreach (var result in largeResults)
            {
                potentialClients.Add(await ExtractPotentialClientDataAsync(result, enhanced, ct));
            }

            return potentialClients;
        }

        private async Task<PotentialClient> ExtractPotentialClientDataAsync(string result, bool enhanced, CancellationToken ct)
        {
            var potentialClient = new PotentialClient()
            {
                Name = ExtractionHelper(result, "<span class=\"h2\">", "<"),
                Address = ExtractionHelper(result, "<address>", "</address>"),
                TelephoneNumber = ExtractionHelper(result, "tel:", "\">"),
                WebsiteUrl = ExtractionHelper(result, "http://", "\""), 
            };

            string interalUrl = ExtractionHelper(result, "<a href=\"/", ".html") + ".html";


            return enhanced ? await PopulateFurtherData(potentialClient, interalUrl, ct) : potentialClient;
        }


        private async Task<PotentialClient> PopulateFurtherData(PotentialClient client, string interalUrl, CancellationToken ct)
        {
            try
            {
                var urlwithDomain = "https://www.solicitors.com/" + interalUrl; // time constraint - appsettings 
                var websiteData = await apiService.GetAsync(urlwithDomain, ct); // arg to seperate into sep loop keep html parse non async

                client.AverageRating = decimal.Parse(ExtractionHelper(websiteData, "Average review score : ", "</div>"));
                client.TotalReviews = decimal.Parse(ExtractionHelper(websiteData, "Total number of reviews : ", "<br>"));
                client.WebsiteUrl = client.WebsiteUrl == "" 
                    ? ExctractWebsiteUrl(ExtractionHelper(websiteData, "links-holder", "website")) 
                    : client.WebsiteUrl;
                return client;
            }
            catch (Exception ex)
            {
                // log exception for visibility
                return client;
            }
        }

        private string ExctractWebsiteUrl(string subsection)
        {
            return ExtractionHelper(subsection, "<a href=\"https://", "\"");
        }

        private string ExtractionHelper(string result, string openingString, string closingString)
        {
            int startIndex = result.IndexOf(openingString);
            if (startIndex == -1) return "";

            startIndex += openingString.Length;

            int endIndex = result.IndexOf(closingString, startIndex);
            if (endIndex == -1) return "";

            return result.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}
