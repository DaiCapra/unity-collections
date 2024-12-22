using Arch.Core;

namespace DataForge.Processors
{
    public interface IComponentProcessor
    {
        bool CanProcess(Entity entity);
        void Process(Entity entity);
    }
}