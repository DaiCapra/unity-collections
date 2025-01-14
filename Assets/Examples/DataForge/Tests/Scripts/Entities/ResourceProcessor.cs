using Arch.Core;
using Arch.Core.Extensions;
using DataForge.Processors;

namespace DataForge.Tests
{
    public class ResourceProcessor : BlueprintProcessor<ResourceBlueprint>
    {
        protected override void OnProcess(Entity entity, ResourceBlueprint blueprint)
        {
            var resource = entity.Get<Resource>();
            resource.amount = blueprint.amount;
            
            entity.Set(resource);
        }
    }
}