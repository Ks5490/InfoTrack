namespace SolSearch.Services.API
{
    public interface IApiService
    {
        Task<string> GetAsync(string url, CancellationToken cancellationToken);
    }
}
