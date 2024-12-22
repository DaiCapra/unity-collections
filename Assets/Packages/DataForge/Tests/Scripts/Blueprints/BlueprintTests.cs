using System.Threading.Tasks;
using DataForge.Blueprints;
using DataForge.ResourcesManagement;
using NUnit.Framework;
using UnityEngine;

namespace DataForge.Tests
{
    public class BlueprintTests
    {
        private const string DefaultLabel = "default";
        private BlueprintManager _blueprintManager;
        private ResourceManager _resourceManager;

        [Test]
        public void CanLoad()
        {
            _blueprintManager.Load<ResourceBlueprint>("Resources");
            Assert.That(_blueprintManager.Blueprints.Count, Is.GreaterThan(0));

            var bp = _blueprintManager.Blueprints["tree"];
            Assert.NotNull(bp);
        }

        [SetUp]
        public async Task Setup()
        {
            _resourceManager = new ResourceManager();
            await _resourceManager.Load(DefaultLabel);

            _blueprintManager = new BlueprintManager(_resourceManager);
        }

        [TearDown]
        public void Teardown()
        {
        }
    }
}