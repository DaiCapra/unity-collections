using System.Collections.Generic;
using Arch.Core;

namespace DataForge.Data
{
    public interface IEntityDataService
    {
        ulong EntityIdentifier { get; set; }
        Dictionary<ulong, Entity> Entities { get; set; }
    }
}