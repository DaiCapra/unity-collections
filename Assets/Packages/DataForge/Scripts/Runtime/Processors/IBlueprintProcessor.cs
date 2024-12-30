using Arch.Core;
using DataForge.Blueprints;

namespace DataForge.Processors
{
    public interface IBlueprintProcessor
    {
        bool CanProcess(Blueprint blueprint);
        void Process(Entity entity, Blueprint blueprint);
        void Restore(Entity entity, Blueprint blueprint);
    }
}