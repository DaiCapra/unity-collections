using System.Collections.Generic;
using System.Threading.Tasks;
using Arch.Core.Extensions;
using DataForge.Blueprints;
using DataForge.Data;
using DataForge.Entities;
using DataForge.Objects;
using DataForge.ResourcesManagement;
using Newtonsoft.Json;
using UnityEngine;

namespace DataForge.Examples
{
    public class DataForgeTests : MonoBehaviour
    {
        private async void Start()
        {
            var resourceManager = new ResourceManager();
            await resourceManager.Load("default");

            var blueprintManager = new BlueprintManager(resourceManager);
            blueprintManager.LoadBlueprints<ResourceBlueprint>("Resources");
            var treeBlueprint = blueprintManager.Blueprints["Tree"] as ResourceBlueprint;

            var objectManager = new ObjectManager();

            var em = new EntityManager(resourceManager, blueprintManager, objectManager);
            em.AddBlueprintProcessor(new ResourceProcessor());
            em.CreateWorld();
            
            var entity = em.Create<ResourceArchetype>(treeBlueprint);
            entity.SetPosition(new Vector3(2, 0, 0));
            
            em.Spawn(entity);
            
            var data = em.Backup(entity);

            await Task.Delay(100);
            em.DestroyAllEntities();
            

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            };
            
            var json = JsonConvert.SerializeObject(data, settings);
            var loadedData = JsonConvert.DeserializeObject<List<object>>(json, settings);

            var e = em.CreateEmptyEntity();
            em.Restore(e, loadedData);
            em.Spawn(e);
        }
    }
}