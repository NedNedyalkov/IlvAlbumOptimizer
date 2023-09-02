using IlvAlbumOptimizer.Illuvidex.Objects;
using IlvAlbumOptimizer.Utils;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Threading.Tasks;

namespace IlvAlbumOptimizer.Illuvidex.API
{
    public partial class IlluvidexClient
    {
        private ApiCall GetAlbumApi = new("https://api.illuvium-game.io/gamedata/illuvitars/album/collections/", Method.Get);

        public async Task<Album> FetchAlbum()
        {
            Logger.Write($"Fetching album... ");

            var response = await CallApi(GetAlbumApi);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = ParseAlbum(response.Content ?? string.Empty);
                Logger.WriteLine($"Done! Total Collections: {result?.CollectionIds?.Count}");
                return result;
            }

            Logger.WriteLine($"{response.StatusCode} {response.ErrorMessage}");
            return null;
        }

        private static Album ParseAlbum(string response)
        {
            var album = new Album();
            if (string.IsNullOrEmpty(response))
                return album;

            if (JObject.Parse(response) is not JObject json)
                return album;

            if (json?["entries"]?.Children() is not JEnumerable<JToken> collections)
                return album;

            foreach (var collection in collections)
            {
                if (collection?["collectionId"]?.ToString() is string collectionId)
                    album.CollectionIds.Add(collectionId);
            }

            return album;
        }
    }
}