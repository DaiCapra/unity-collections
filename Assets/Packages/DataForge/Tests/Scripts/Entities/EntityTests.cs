using System.Threading.Tasks;
using Arch.Core.Extensions;
using DataForge.Blueprints;
using DataForge.Data;
using DataForge.Entities;
using DataForge.Objects;
using DataForge.ResourcesManagement;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DataForge.Tests
{
    public class EntityTests
    {
        private const string DefaultLabel = "default";

        private EntityManager _em;
        private ResourceBlueprint _treeBlueprint;

        [Test]
        public void CanBackupAndRestore()
        {
            var e1 = _em.Create<ResourceArchetype>(blueprint: _treeBlueprint);
            e1.Change((ref Resource r) => { r.amount = 50; });

            var components = _em.Backup(e1);

            var e2 = _em.CurrentWorld.Create();
            Assert.False(e1.AreComponentsSame(e2));

            _em.Restore(e2, components);
            Assert.True(e1.AreComponentsSame(e2));

            var s1 = JsonConvert.SerializeObject(e1.GetAllComponents());
            var s2 = JsonConvert.SerializeObject(e2.GetAllComponents());
            Assert.True(string.Equals(s1, s2));
        }

        [Test]
        public void CanCreateResource()
        {
            var entity = _em.Create<ResourceArchetype>(blueprint: _treeBlueprint);
            Assert.That(entity.Has<STransform>());
            Assert.That(entity.Has<Resource>());

            var resource = entity.Get<Resource>();
            Assert.That(resource.amount, Is.EqualTo(_treeBlueprint.amount));

            var transform = entity.Get<STransform>();
            Assert.That(transform.scale, Is.EqualTo(SVector3.One));
        }

        [Test]
        public void CanSpawn()
        {
            var entity = _em.Create<ResourceArchetype>(blueprint: _treeBlueprint);
            _em.Spawn(entity);
        }

        [SetUp]
        public async Task Setup()
        {
            var resourceManager = new ResourceManager();
            await resourceManager.Load(DefaultLabel);

            var blueprintManager = new BlueprintManager(resourceManager);
            blueprintManager.Load<ResourceBlueprint>("Resources");

            var objectManager = new ObjectManager();

            _em = new EntityManager(resourceManager, blueprintManager, objectManager);
            _em.AddBlueprintProcessor(new ResourceProcessor());
            _em.CreateWorld();

            _treeBlueprint = blueprintManager.Blueprints["Tree"] as ResourceBlueprint;
        }

        [TearDown]
        public void Teardown()
        {
            _em.DestroyWorlds();
        }
    }
}