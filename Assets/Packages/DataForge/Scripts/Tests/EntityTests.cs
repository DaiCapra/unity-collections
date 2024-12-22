using Arch.Core.Extensions;
using DataForge.Data;
using DataForge.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

namespace DataForge.Tests
{
    public class EntityTests
    {
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

        [SetUp]
        public void Setup()
        {
            _em = new EntityManager();
            _em.AddBlueprintProcessor(new ResourceProcessor());

            _em.CreateWorld();

            _treeBlueprint = new ResourceBlueprint()
            {
                id = "tree",
                amount = 100
            };
        }

        [TearDown]
        public void Teardown()
        {
            _em.DestroyWorlds();
        }
    }
}