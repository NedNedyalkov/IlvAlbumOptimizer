using IlvAlbumOptimizer.Illuvidex.API;
using IlvAlbumOptimizer.Illuvidex.Objects;
using IlvAlbumOptimizer.Utils;
using System.Threading.Tasks;

namespace IlvAlbumOptimizer.Illuvidex
{
    public class AlbumUnsleever
    {
        public AlbumUnsleever(string token, bool dryRun)
        {
            IlluvidexClient = new IlluvidexClient(token);
            DryRun = dryRun;
        }

        private IlluvidexClient IlluvidexClient { get; }
        public bool DryRun { get; }

        private int totalIlluvitarsUnsleeved = 0;

        public async Task UnsleeveAlbum()
        {
            Logger.WriteLine($"Started Unsleeving!");

            var album = await IlluvidexClient.FetchAlbum();

            foreach (var collectionId in album?.CollectionIds)
                await UnsleeveCollectionImpl(collectionId);

            Logger.WriteLine($"Finished! Total Illuvitars unsleeved: {totalIlluvitarsUnsleeved}");
        }

        public async Task UnsleeveCollection(string collectionId)
        {
            Logger.WriteLine($"Started Unsleeving!");
            await UnsleeveCollectionImpl(collectionId);
            Logger.WriteLine($"Finished! Total Illuvitars unsleeved: {totalIlluvitarsUnsleeved}");
        }

        private async Task UnsleeveCollectionImpl(string collectionId)
        {
            var collection = await IlluvidexClient.FetchCollection(collectionId);
            Logger.Write($"Unsleeving {collection}... ", isVerbose: true);

            foreach (var sleeve in collection.Sleeves)
                await Unsleeve(collectionId, sleeve);
        }

        public async Task Unsleeve(string collectionId, Sleeve sleeve)
        {
            if (!sleeve.HasSleevedIlluvitar)
            {
                Logger.WriteLine($"No Illuvitar in slot {sleeve.Id}...", isVerbose: true);
                return;
            }

            totalIlluvitarsUnsleeved++;
            Logger.WriteLine($"{sleeve.SleevedIlluvitar} Unsleeved!");

            if (!DryRun)
            {
                await IlluvidexClient.Unsleeve(collectionId, sleeve.Id);
            }
        }
    }
}