using IlvAlbumOptimizer.Utils;
using RestSharp;
using System.Threading.Tasks;

namespace IlvAlbumOptimizer.Illuvidex.API
{
    public partial class IlluvidexClient
    {
        public async Task<string> Sleeve(string collection, int slot, int tokenId)
        {
            var client = CreateRestClient($"https://api.illuvium-game.io/gamedata/illuvitars/album/collection/{collection}/sleeve/{slot}");
            var request = CreateRequest(Method.Put);
            request.AddHeader("Content-Type", "text/plain");
            request.AddParameter("text/plain", $@"{{""tokenId"":{tokenId}}}", ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            Logger.WriteLine($"Sleeving result: {response.StatusCode} {response.ErrorMessage}");

            return response.Content ?? string.Empty;
        }
    }
}