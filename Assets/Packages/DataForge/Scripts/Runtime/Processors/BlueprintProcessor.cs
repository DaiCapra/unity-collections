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

        public void Restore(Entity entity, Blueprint blueprint)
        {
            if (blueprint is T t)
            {
                OnRestore(entity, t);
            }
        }

        protected virtual void OnProcess(Entity entity, T blueprint)
        {
        }

        protected virtual void OnRestore(Entity entity, T blueprint)
        {
        }
    }
}