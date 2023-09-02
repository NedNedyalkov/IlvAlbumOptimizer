using RestSharp;
using System;
using System.Threading.Tasks;

namespace IlvAlbumOptimizer.Illuvidex.API
{
    public partial class IlluvidexClient
    {
        public IlluvidexClient(string bearerToken)
            => BearerToken = bearerToken.Replace("Bearer ", string.Empty, StringComparison.InvariantCultureIgnoreCase);

        public string BearerToken { get; }

        private async Task<RestResponse> CallApi(ApiCall apiCall, params object[] apiParameters)
        {
            var client = CreateRestClient(string.Format(apiCall.Api, apiParameters));
            var request = CreateRequest(apiCall.Method);
            var response = await client.ExecuteAsync(request);
            return response;
        }

        private static RestClient CreateRestClient(string api)
        {
            var client = new RestClient(api);
            client.Options.MaxTimeout = 3;
            return client;
        }

        private RestRequest CreateRequest(Method method)
        {
            var request = new RestRequest(string.Empty, method);
            request.AddHeader("Authorization", $"Bearer {BearerToken}");
            return request;
        }

        private record struct ApiCall(string Api, Method Method);
    }
}