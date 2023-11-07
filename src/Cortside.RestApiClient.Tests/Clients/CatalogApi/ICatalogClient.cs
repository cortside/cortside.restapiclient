using System.Threading.Tasks;
using RestSharp;

namespace Cortside.RestApiClient.Tests.Clients.CatalogApi
{
    public interface ICatalogClient
    {
        Task<RestResponse<CatalogItem>> GetItemAsync(string sku);
        Task<RestResponse<CatalogItem>> CreateItemAsync(bool followRedirects);
        Task<RestResponse> SearchItemsAsync(bool followRedirects);
        Task<RestResponse> TemporaryRedirect();
        Task<RestResponse> ModelMismatchAsync();
    }
}
