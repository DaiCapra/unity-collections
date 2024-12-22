using DataForge.Data;
using DataForge.Entities;

namespace DataForge.Examples
{
    public class ResourceArchetype : Archetype
    {
        public ResourceArchetype()
        {
            Add<STransform>();
            Add<Resource>();
        }
    }
}