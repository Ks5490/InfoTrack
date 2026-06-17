namespace SolSearch.Services.API
{
    public class ApiService(HttpClient httpClient) : IApiService
    {
        private readonly HttpClient _httpClient = httpClient;
        public async Task<string> GetAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                // simplified error handling - add logger not return TMI
                throw new HttpRequestException($"Request to {url} failed with {(int)response.StatusCode} : {response.ReasonPhrase}");

            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"An error occurred while making a GET request to {url}.", ex);
            }
        }
    }
}
