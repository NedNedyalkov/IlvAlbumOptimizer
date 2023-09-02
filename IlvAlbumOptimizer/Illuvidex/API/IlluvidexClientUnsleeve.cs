using IlvAlbumOptimizer.Utils;
using RestSharp;
using System.Threading.Tasks;

namespace IlvAlbumOptimizer.Illuvidex.API
{
    public partial class IlluvidexClient
    {
        private ApiCall UnsleeveApi = new("https://api.illuvium-game.io/gamedata/illuvitars/album/collection/{0}/sleeve/{1}", Method.Delete);
        public async Task<string> Unsleeve(string collection, int slot)
        {
            var response = await CallApi(UnsleeveApi, collection, slot);
            Logger.WriteLine($"Unsleeve result: {response.StatusCode} {response.ErrorMessage}");

            return response.Content ?? string.Empty;
        }
    }
}