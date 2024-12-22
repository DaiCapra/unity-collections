using System.Collections.Generic;
using DataForge.Blueprints;
using DataForge.Data;
using UnityEngine;

namespace DataForge.ResourcesManagement
{
    public interface IResourceProvider
    {
        Dictionary<string, Object> Resources { get; }
        GameObject GetPrefab(Blueprint blueprint, BlueprintReference reference);
    }
}