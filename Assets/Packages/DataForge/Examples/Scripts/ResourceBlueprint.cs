using Arch.Core;
using Arch.Core.Extensions;
using DataForge.Blueprints;
using DataForge.Processors;

namespace DataForge.Examples
{
    public class ResourceBlueprint : Blueprint
    {
        public int amount;
    }

    public class ResourceProcessor : BlueprintProcessor<ResourceBlueprint>
    {
        protected override void OnProcess(Entity entity, ResourceBlueprint blueprint)
        {
            entity.Set(new Resource() { amount = blueprint.amount });
        }
    }
}