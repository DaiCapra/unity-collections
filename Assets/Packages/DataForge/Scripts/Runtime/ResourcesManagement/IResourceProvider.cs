using System.Collections.Generic;
using UnityEngine;

namespace DataForge.ResourcesManagement
{
    public interface IResourceProvider
    {
        Dictionary<string, Object> Resources { get; }
    }
}