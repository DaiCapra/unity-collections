using DataForge.Data;
using DataForge.Entities;

namespace DataForge.Tests
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