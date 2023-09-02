using IlvAlbumOptimizer.Illuvidex.Objects;
using IlvAlbumOptimizer.Utils;
using Imx.Sdk;
using Imx.Sdk.Gen.Client;
using Imx.Sdk.Gen.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace IlvAlbumOptimizer.IMX
{
    public class ImxIlluvitars
    {
        private const string IlluvitarsCollection = "0x8cceea8cfb0f8670f4de3a6cd2152925605d19a8";

        public static IEnumerable<Illuvitar> ReadFromWallet(string wallet)
        {
            Logger.Write($"Fetching wallet illuvitars... ");

            var client = new Client(new Config() { Environment = EnvironmentSelector.Mainnet });
            var cursor = string.Empty;
            var result = new List<Illuvitar>();

            try
            {
                while (true)
                {
                    var resultListAssets = client.ListAssets(collection: IlluvitarsCollection, user: wallet, pageSize: 100000, cursor: cursor);

                    if (resultListAssets == null)
                        break;

                    var illuvitars = resultListAssets.Result.Select(IlluvitarTransformer);
                    result.AddRange(illuvitars);

                    if (resultListAssets.Remaining <= 0 || cursor == resultListAssets.Cursor)
                        break;

                    cursor = resultListAssets.Cursor;
                }
            }
            catch (ApiException e)
            {
                Logger.WriteLine("Exception: " + e.Message);
                Logger.WriteLine("Status Code: " + e.ErrorCode);
            }

            Logger.WriteLine($"Done! Total Illuvitars: {result.Count}");
            return result;
        }

        static Illuvitar IlluvitarTransformer(AssetWithOrders asset)
        {
            var result = new Illuvitar() { Id = int.Parse(asset.TokenId) };
            if (asset.Metadata is not JObject json)
                return result;

            result.MetaData = json;

            if (json["name"]?.Value<string>() is string name)
                result.Name = name;

            if (json["Set"]?.Value<int>() is int set)
                result.Set = set;

            if (json["Wave"]?.Value<int>() is int wave)
                result.Wave = wave;

            if (json["Production Number"]?.Value<int>() is int productionNumber)
                result.ProductionNumber = productionNumber;

            if (json["Total Power"]?.Value<int>() is int totalPower)
                result.TotalPower = totalPower;

            return result;
        }

        public static IEnumerable<Illuvitar> FilterOnlyTopIlluvitars(IEnumerable<Illuvitar> illuvitars)
        {
            var groupedByProductionId = illuvitars.GroupBy(illuvitar => illuvitar.ProductionNumber)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.OrderByDescending(illuvitar => illuvitar.TotalPower).First());
            var onlyTheBest = groupedByProductionId.Select(kvp => kvp.Value).OrderBy(illuvitar => illuvitar.ProductionNumber);
            return onlyTheBest;
        }
    }
}