﻿using System;
using System.Collections.Generic;
using DataForge.ResourcesManagement;
using Newtonsoft.Json;
using UnityEngine;

namespace DataForge.Blueprints
{
    public class BlueprintManager : IBlueprintManager
    {
        private readonly IResourceManager _manager;

        public BlueprintManager(IResourceManager manager)
        {
            _manager = manager;
        }

        public Dictionary<string, Blueprint> Blueprints { get; } = new(StringComparer.OrdinalIgnoreCase);

        public void Clear()
        {
            Blueprints.Clear();
        }

        public virtual void Load()
        {
        }

        public void LoadBlueprints<T>(string key) where T : Blueprint
        {
            var bps = LoadData<T>(key);
            foreach (var bp in bps)
            {
                if (string.IsNullOrEmpty(bp?.id))
                {
                    Debug.LogError($"Missing blueprint id!");
                    continue;
                }

                Blueprints[bp.id] = bp;
            }
        }

        private T[] LoadData<T>(string key) where T : Blueprint
        {
            if (!_manager.Resources.TryGetValue(key, out var resource))
            {
                Debug.LogError($"Resource not found: {key}");
                return Array.Empty<T>();
            }

            var textAsset = resource as TextAsset;
            if (textAsset == null)
            {
                Debug.LogError($"Blueprint needs to be text asset: {key}");
                return Array.Empty<T>();
            }

            var data = JsonConvert.DeserializeObject<T[]>(textAsset.text);
            return data;
        }
    }
}