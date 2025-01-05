// using DataForge.Tests;

using DataForge.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace DataForge.Examples
{
    public class NewMonoBehaviourScript : MonoBehaviour
    {
        private const string DefaultLabel = "default";

        private async void Start()
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            // var v1 = new Vector3(42, 42, 42);
            var v1 = new STransform();
            var json = JsonConvert.SerializeObject(v1, jsonSettings);
            Debug.Log(json);

            var v2 = JsonConvert.DeserializeObject<STransform>(json, jsonSettings);

            /*
            var resourceManager = new ResourceManager();
            await resourceManager.Load(DefaultLabel);

            var blueprintManager = new BlueprintManager(resourceManager);
            blueprintManager.Load<ResourceBlueprint>("Resources");

            var objectManager = new ObjectManager();

            var em = new EntityManager(resourceManager, blueprintManager, objectManager, new EntityDataService());
            em.AddBlueprintProcessor(new ResourceProcessor());
            em.CreateWorld();

            var treeBlueprint = blueprintManager.Blueprints["Tree"] as ResourceBlueprint;
            var e = em.Create<ResourceArchetype>(treeBlueprint);
            var g = em.Spawn(e);

            Selection.activeObject = g;*/
        }
    }
}