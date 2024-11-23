using IlvAlbumOptimizer.Illuvidex.API;
using IlvAlbumOptimizer.Illuvidex.Objects;
using IlvAlbumOptimizer.IMX;
using IlvAlbumOptimizer.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IlvAlbumOptimizer.Illuvidex
{
    public class AlbumOptimizer
    {
        public AlbumOptimizer(string token, string wallet, bool dryRun)
        {
            IlluvidexClient = new IlluvidexClient(token);
            Wallet = wallet;
            DryRun = dryRun;
        }

        private IlluvidexClient IlluvidexClient { get; }
        private IEnumerable<Illuvitar> IlluvitarsInWallet { get; set; }
        public string Wallet { get; }
        public bool DryRun { get; }

        private int totalIlluvitarsOptimized = 0;
        private int totalPowerOptimized = 0;

        public async Task OptimizeAlbum()
        {
            Logger.WriteLine($"Started Optimization!");
            LoadWalletIlluvitars();

            var album = await IlluvidexClient.FetchAlbum();

            if (album is null)
            {
                Logger.WriteLine($"Failed to load illuvitars from Illuvidex");
                return;
            }

            foreach (var collectionId in album?.CollectionIds)
                await OptimizeCollectionImpl(collectionId);

            Logger.WriteLine($"Finished! New Illuvitars sleeved: {totalIlluvitarsOptimized} Total Power Increased: {totalPowerOptimized}");
        }

        public async Task OptimizeCollection(string collectionId)
        {
            Logger.WriteLine($"Started Optimization!");
            LoadWalletIlluvitars();
            await OptimizeCollectionImpl(collectionId);
            Logger.WriteLine($"Finished! New Illuvitars sleeved: {totalIlluvitarsOptimized} Total Power Increased: {totalPowerOptimized}");
        }

        private async Task OptimizeCollectionImpl(string collectionId)
        {
            var collection = await IlluvidexClient.FetchCollection(collectionId);
            if (collection is null)
            {
                Logger.WriteLine($"Failed to load collection from Illuvidex");
                return;
            }

            foreach (var sleeve in collection.Sleeves)
            {
                Logger.Write($"Optimizing {collection} slot {sleeve.Id}... ", isVerbose: true);

                if (!sleeve.IsReleased)
                {
                    Logger.WriteLine($"Not released yet!", isVerbose: true);
                    continue;
                }

                var bestIlluvitar = FindBestIlluvitarForSleeve(IlluvitarsInWallet, sleeve);
                await Sleeve(collectionId, bestIlluvitar, sleeve);
            }
        }

        public async Task Sleeve(string collectionId, Illuvitar illuvitar, Sleeve sleeve)
        {
            if (illuvitar is null)
            {
                Logger.WriteLine($"Not owned!", isVerbose: true);
                return;
            }

            if (sleeve.SleevedIlluvitar.Id == illuvitar.Id)
            {
                Logger.WriteLine($"{sleeve.SleevedIlluvitar} Already sleeved!", isVerbose: true);
                return;
            }

            totalIlluvitarsOptimized++;
            totalPowerOptimized += illuvitar.TotalPower - sleeve.SleevedIlluvitar.TotalPower;
            Logger.WriteLine($"{sleeve.SleevedIlluvitar} => {illuvitar}!");

            if (!DryRun)
            {
                await IlluvidexClient.Sleeve(collectionId, sleeve.Id, illuvitar.Id);
            }
        }

        private static Illuvitar FindBestIlluvitarForSleeve(IEnumerable<Illuvitar> illuvitars, Sleeve sleeve)
        {
            var matchingIlluvitars = illuvitars.Where(illuvitar => IsIlluvitarForSleeve(illuvitar, sleeve));
            var orderedByPower = matchingIlluvitars.OrderByDescending(OrderIlluvitarsByPower);
            return orderedByPower.FirstOrDefault();

            static int OrderIlluvitarsByPower(Illuvitar illuvitar) => illuvitar.TotalPower;
        }

        private static bool IsIlluvitarForSleeve(Illuvitar illuvitar, Sleeve sleeve)
        {
            if (sleeve.MetaData is not null && illuvitar.MetaData is not null)
            {
                foreach (var sleeveNode in sleeve.MetaData)
                {
                    var key = sleeveNode.Key;
                    if (ConditionToPropertyMap.TryGetValue(sleeveNode.Key, out var mappedKey))
                        key = mappedKey;

                    illuvitar.MetaData.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var illuvitarProperty);
                    if (illuvitarProperty is null)
                        return false;

                    sleeve.MetaData.TryGetValue(sleeveNode.Key, StringComparison.OrdinalIgnoreCase, out var sleeveCondition);
                    if (sleeveCondition is null)
                        return false;

                    if (illuvitarProperty.ToString() != sleeveCondition.ToString())
                        return false;
                }
            }

            return true;
        }

        private void LoadWalletIlluvitars() => IlluvitarsInWallet = ImxIlluvitars.ReadFromWallet(Wallet);

        private static Dictionary<string, string> ConditionToPropertyMap = new Dictionary<string, string>()
        {
            { "HeadLine", "Headwear Line"},
            { "HeadStage", "Headwear Stage"},
            { "BodyLine", "Bodywear Line"},
            { "BodyStage", "Bodywear Stage"},
            { "EyesLine", "Eyewear Line"},
            { "EyesStage", "Eyewear Stage"},
            { "PropLine", "Prop Line"},
            { "PropStage", "Prop Stage"},
            { "SkinLine", "Skin Line"},
            { "SkinStage", "Skin Stage"},

            { "BackgroundLine", "Background Line"},
            { "BackgroundStage", "Background Stage"},

            { "ProductionNumber", "Production Number"},
            { "ExpressionRarity", "Expression Rarity"},
        };
    }
}