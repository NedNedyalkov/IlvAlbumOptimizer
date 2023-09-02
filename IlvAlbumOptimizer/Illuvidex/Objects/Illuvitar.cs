using Newtonsoft.Json.Linq;

namespace IlvAlbumOptimizer.Illuvidex.Objects
{
    public class Illuvitar
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Set { get; set; } = 1;
        public int Wave { get; set; }
        public int ProductionNumber { get; set; }
        public int TotalPower { get; set; }
        public JObject MetaData { get; internal set; } = new JObject();

        public override string ToString() => $"{Name} ({Id}) = {TotalPower}";
    }
}