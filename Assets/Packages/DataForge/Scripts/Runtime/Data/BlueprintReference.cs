using DataForge.Blueprints;
using Newtonsoft.Json;

namespace DataForge.Data
{
    public struct BlueprintReference
    {
        public string blueprintId;
        public int prefabIndex;
        [JsonIgnore] public Blueprint blueprint;
    }
}