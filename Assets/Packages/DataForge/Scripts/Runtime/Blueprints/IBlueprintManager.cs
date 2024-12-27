using System.Collections.Generic;

namespace DataForge.Blueprints
{
    public interface IBlueprintManager
    {
        Dictionary<string, Blueprint> Blueprints { get; set; }
        void Load<T>(string key) where T : Blueprint;
    }
}