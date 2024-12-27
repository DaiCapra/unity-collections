using System.Collections.Generic;
using System.Threading.Tasks;
using DataForge.Blueprints;
using DataForge.Data;
using UnityEngine;

namespace DataForge.ResourcesManagement
{
    public interface IResourceManager
    {
        GameObject GetPrefab(Blueprint blueprint, BlueprintReference reference);
        Dictionary<string, Object> Resources { get; set; }
        Task Load(string label);
    }
}