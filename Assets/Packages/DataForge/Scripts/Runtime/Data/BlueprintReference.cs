using DataForge.Blueprints;
using Newtonsoft.Json;

namespace DataForge.Data
{
    public struct BlueprintReference
    {
        public string blueprintId;
        public int resourceIndex;
        [JsonIgnore] public Blueprint blueprint;
    }
}