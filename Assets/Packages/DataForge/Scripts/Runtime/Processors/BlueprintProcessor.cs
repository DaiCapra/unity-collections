using Arch.Core;
using DataForge.Blueprints;

namespace DataForge.Processors
{
    public abstract class BlueprintProcessor<T> : IBlueprintProcessor
        where T : Blueprint
    {
        public void Process(Entity entity, Blueprint blueprint)
        {
            if (blueprint is T t)
            {
                OnProcess(entity, t);
            }
        }

        public bool CanProcess(Blueprint blueprint)
        {
            return blueprint is T;
        }

        protected abstract void OnProcess(Entity entity, T blueprint);
    }
}