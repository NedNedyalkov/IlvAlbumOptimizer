using IlvAlbumOptimizer.Illuvidex.API;
using IlvAlbumOptimizer.Illuvidex.Objects;
using IlvAlbumOptimizer.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IlvAlbumOptimizer.Illuvidex
{
    public class AlbumGetBindingIdeas
    {
        public AlbumGetBindingIdeas(string token) => IlluvidexClient = new IlluvidexClient(token);

        private IlluvidexClient IlluvidexClient { get; }
        public List<BindingIdea> BindingIdeasList { get; } = new List<BindingIdea>();
        private Dictionary<int, BindingIdea> IlluvitarIdToOccurencesInAlbum { get; } = new Dictionary<int, BindingIdea>();

        public async Task AnalyzeBindingPossibilities()
        {
            var album = await IlluvidexClient.FetchAlbum();

            if (album is null)
            {
                Logger.WriteLine($"Failed to load illuvitars from Illuvidex");
                return;
            }

            foreach (var collectionId in album?.CollectionIds)
            {
                var collection = await IlluvidexClient.FetchCollection(collectionId);
                if (collection is null)
                {
                    Logger.WriteLine($"Failed to load collection from Illuvidex");
                    return;
                }

                foreach (var sleeve in collection.Sleeves)
                {
                    if (sleeve.HasSleevedIlluvitar && !sleeve.SleevedIlluvitar.HasAccessories)
                    {
                        if (!IlluvitarIdToOccurencesInAlbum.ContainsKey(sleeve.SleevedIlluvitar.Id))
                            IlluvitarIdToOccurencesInAlbum[sleeve.SleevedIlluvitar.Id] = new BindingIdea(sleeve.SleevedIlluvitar, 0);

                        var bindingIdea = IlluvitarIdToOccurencesInAlbum[sleeve.SleevedIlluvitar.Id];
                        bindingIdea.Occurences++;
                        IlluvitarIdToOccurencesInAlbum[sleeve.SleevedIlluvitar.Id] = bindingIdea;
                    }
                }
            }

            Sort();
        }

        private void Sort()
            => IlluvitarIdToOccurencesInAlbum
            .OrderByDescending(x => x.Value.Occurences)
            .ToDictionary(x => x.Key, x => x.Value)
            .ToList()
            .ForEach(x =>
            {
                BindingIdeasList.Add(x.Value);
            });

        public record struct BindingIdea(Illuvitar Illuvitar, int Occurences);
    }
}