using System.Collections.Generic;
using System.Threading.Tasks;
using DataForge.Blueprints;
using DataForge.Data;
using UnityEngine;

namespace DataForge.ResourcesManagement
{
    public interface IResourceManager
    {
        Dictionary<string, Object> Resources { get; }
        GameObject GetPrefab(Blueprint blueprint, BlueprintReference reference);
        Task Load(string label);
    }
}