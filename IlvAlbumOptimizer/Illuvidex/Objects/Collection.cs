using System.Collections.Generic;

namespace IlvAlbumOptimizer.Illuvidex.Objects
{
    public class Collection
    {
        public required string Id { get; set; }
        public List<Sleeve> Sleeves { get; set; } = new List<Sleeve>();

        public override string ToString() => $"{Id}";
    }
}