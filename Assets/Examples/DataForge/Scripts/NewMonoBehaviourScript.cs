using System.Threading.Tasks;
using DataForge.Blueprints;
using DataForge.Entities;
using DataForge.Objects;
using DataForge.ResourcesManagement;
using DataForge.Tests;
using UnityEditor;
using UnityEngine;

namespace DataForge.Examples
{
    public class NewMonoBehaviourScript : MonoBehaviour
    {
        private const string DefaultLabel = "default";

        private async void Start()
        {
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

            Selection.activeObject = g;
        }
    }
}