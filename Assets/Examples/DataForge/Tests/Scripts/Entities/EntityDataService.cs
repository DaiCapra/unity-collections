using Arch.Core;
using DataForge.Data;

namespace DataForge.Tests
{
    public class EntityDataService : IEntityDataService
    {
        public Map<Entity> Entities { get; set; } = new();
    }
}