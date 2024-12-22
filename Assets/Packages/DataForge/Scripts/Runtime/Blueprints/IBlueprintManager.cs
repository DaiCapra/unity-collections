using System.Collections.Generic;

namespace DataForge.Blueprints
{
    public interface IBlueprintManager
    {
        Dictionary<string, Blueprint> Blueprints { get; }
        void Load();
    }
}