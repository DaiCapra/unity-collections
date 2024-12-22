using Arch.Core;
using Arch.Core.Extensions;

namespace DataForge.Processors
{
    public abstract class ComponentProcessor<T> : IComponentProcessor
    {
        public bool CanProcess(Entity entity)
        {
            return entity.Has<T>();
        }

        public void Process(Entity entity)
        {
            var t = entity.Get<T>();
            OnProcess(entity, ref t);
            entity.Set(t);
        }

        public abstract void OnProcess(Entity entity, ref T t);
    }
}