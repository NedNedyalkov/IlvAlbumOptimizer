using System.Collections.Generic;

namespace IlvAlbumOptimizer.Illuvidex.Objects
{
    public class Album
    {
        public List<string> CollectionIds { get; set; } = new List<string>();

        public override string ToString() => $"Album with {CollectionIds.Count} collections";
    }
}