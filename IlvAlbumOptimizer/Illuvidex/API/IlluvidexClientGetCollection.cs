using IlvAlbumOptimizer.Illuvidex.Objects;
using IlvAlbumOptimizer.Utils;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Threading.Tasks;
using System.Web;

namespace IlvAlbumOptimizer.Illuvidex.API
{
    public partial class IlluvidexClient
    {

        private ApiCall GetCollectionApi = new("https://api.illuvium-game.io/gamedata/illuvitars/album/collection/{0}", Method.Get);
        public async Task<Collection> FetchCollection(string collection)
        {
            Logger.Write($"Collection: {collection}... ");

            var response = await CallApi(GetCollectionApi, collection);
            Logger.Write($"Result: {response.StatusCode} {response.ErrorMessage}... ");

            var result = ParseCollection(response.Content ?? string.Empty);
            Logger.WriteLine($"Sleeves: {result.Sleeves.Count}");

            return result;
        }

        private static Collection ParseCollection(string response)
        {
            var collection = new Collection() { Id = string.Empty };

            if (string.IsNullOrEmpty(response))
                return collection;

            if (JObject.Parse(response) is not JObject json)
                return collection;

            if (json["collectionId"]?.Value<string>() is not string collectionId)
                return collection;

            collection.Id = collectionId;

            if (json["sleeves"]?.Children() is not JEnumerable<JToken> sleeves)
                return collection;

            foreach (var sleeve in sleeves)
                ParseSleeve(collection, sleeve);

            return collection;
        }

        private static void ParseSleeve(Collection collection, JToken sleeveJson)
        {
            var sleeve = new Sleeve();

            if (sleeveJson?["id"]?.Value<int>() is int sleeveId)
                sleeve.Id = sleeveId;

            if (sleeveJson?["isReleased"]?.Value<bool>() is bool isReleased)
                sleeve.IsReleased = isReleased;

            if (sleeveJson?["imxMetadataFilter"]?.Value<string>() is string filter)
            {
                var metaData = HttpUtility.UrlDecode(filter);
                if (JObject.Parse(metaData) is JObject meta)
                {
                    sleeve.MetaData = meta;
                }
            }

            ParseAlreadySleevedIlluvitar(sleeveJson, sleeve);

            collection.Sleeves.Add(sleeve);
        }

        private static void ParseAlreadySleevedIlluvitar(JToken sleeveJson, Sleeve sleeve)
        {
            if (sleeveJson?["token"]?.Value<JToken>() is not JToken sleevedIlluvitar)
                return;

            if (sleevedIlluvitar["id"]?.Value<int>() is int sleevedIlluvitarId)
                sleeve.SleevedIlluvitar.Id = sleevedIlluvitarId;

            if (sleevedIlluvitar["name"]?.Value<string>() is string sleevedIlluvitarName)
                sleeve.SleevedIlluvitar.Name = sleevedIlluvitarName;

            if (sleevedIlluvitar["totalPower"]?.Value<int>() is int sleevedIlluvitarPower)
                sleeve.SleevedIlluvitar.TotalPower = sleevedIlluvitarPower;

            if (sleevedIlluvitar["accessoryPower"]?.Value<int>() is int sleevedIlluvitarAccessoryPower)
                sleeve.SleevedIlluvitar.AccessoryPower = sleevedIlluvitarAccessoryPower;
        }
    }
}