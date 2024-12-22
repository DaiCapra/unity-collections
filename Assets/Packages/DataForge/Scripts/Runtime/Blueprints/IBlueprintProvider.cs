using System.Collections.Generic;

namespace DataForge.Blueprints
{
    public interface IBlueprintProvider
    {
        Dictionary<string, Blueprint> Blueprints { get; }
    }
}