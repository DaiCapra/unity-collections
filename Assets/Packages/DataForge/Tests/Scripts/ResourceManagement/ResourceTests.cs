﻿using System.Threading.Tasks;
using DataForge.ResourcesManagement;
using NUnit.Framework;
using UnityEngine;

namespace DataForge.Tests
{
    public class ResourceTests
    {
        private const string DefaultLabel = "default";
        private ResourceManager _resourceManager;

        [Test]
        public async Task CanLoadAddressables()
        {
            await _resourceManager.Load(DefaultLabel);
            
            var resources = _resourceManager.Resources;

            Assert.That(resources.Count, Is.GreaterThan(0));
            Debug.Log($"Resources: {resources.Count}");
        }

        [SetUp]
        public void Setup()
        {
            _resourceManager = new ResourceManager();
        }

        [TearDown]
        public void Teardown()
        {
        }
    }
}