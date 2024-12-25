using System.Collections.Generic;
using Arch.Core;
using DataForge.Data;

namespace DataForge.Tests
{
    public class EntityDataService : IEntityDataService
    {
        public ulong EntityIdentifier { get; set; } = 1;
        public Dictionary<ulong, Entity> Entities { get; set; } = new();
    }
}