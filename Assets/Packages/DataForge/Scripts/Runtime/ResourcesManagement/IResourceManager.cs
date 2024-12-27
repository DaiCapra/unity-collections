using System.Collections.Generic;
using System.Threading.Tasks;
using DataForge.Blueprints;
using DataForge.Data;
using UnityEngine;

namespace DataForge.ResourcesManagement
{
    public interface IResourceManager
    {
        Dictionary<string, Object> Resources { get; set; }
        GameObject GetPrefab(Blueprint blueprint, BlueprintReference reference);
        GameObject GetPrefab(string key);
        Task Load(string label);
    }
}