using Newtonsoft.Json.Linq;

namespace IlvAlbumOptimizer.Illuvidex.Objects
{
    public class Sleeve
    {
        public int Id { get; set; }
        public bool IsReleased { get; set; }
        public Illuvitar SleevedIlluvitar { get; set; } = new Illuvitar();
        public JObject MetaData { get; internal set; } = new JObject();
        public bool HasSleevedIlluvitar => SleevedIlluvitar.Id > 0;

        public override string ToString() => $"Id: {Id} {(SleevedIlluvitar is not null ? $"Sleeved: {SleevedIlluvitar}" : string.Empty)}";
    }
}