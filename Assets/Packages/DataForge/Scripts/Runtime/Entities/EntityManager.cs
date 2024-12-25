using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.Core.Utils;
using DataForge.Blueprints;
using DataForge.Data;
using DataForge.Objects;
using DataForge.Processors;
using DataForge.ResourcesManagement;
using UnityEngine;

namespace DataForge.Entities
{
    public class EntityData
    {
        public List<object> components = new();
        public ulong id;
    }

    public class EntityManager
    {
        private readonly Dictionary<Type, Archetype> _archetypes = new();
        private readonly BlueprintManager _blueprintManager;
        private readonly List<IBlueprintProcessor> _blueprintProcessors = new();
        private readonly List<IComponentProcessor> _componentProcessors = new();
        private readonly IObjectManager _objectManager;
        private readonly ResourceManager _resourceManager;

        public EntityManager(
            ResourceManager resourceManager,
            BlueprintManager blueprintManager,
            IObjectManager objectManager
        )
        {
            _blueprintManager = blueprintManager;
            _resourceManager = resourceManager;
            _objectManager = objectManager;

            AddComponentProcessor(new STransformProcessor());
        }

        public Dictionary<Entity, Actor> Actors { get; set; } = new();

        public ulong Identity { get; set; } = 1;
        public World CurrentWorld => World.Worlds[0];

        public void AddBlueprintProcessor(IBlueprintProcessor processor)
        {
            _blueprintProcessors.Add(processor);
        }

        public void AddComponentProcessor(IComponentProcessor processor)
        {
            _componentProcessors.Add(processor);
        }

        public EntityData Backup(Entity entity)
        {
            var components = entity.GetAllComponents()
                .Where(t => t is not ITransientData)
                .ToList();

            var data = new EntityData
            {
                components = components,
                id = entity.GetId()
            };

            return data;
        }

        public Entity Create<T>(Blueprint blueprint = null) where T : Archetype
        {
            var type = typeof(T);
            return Create(type, blueprint);
        }

        public Entity Create(Type archetypeType, Blueprint blueprint = null)
        {
            var archetype = EnsureArchetype(archetypeType);

            var types = archetype.GetComponentTypes();
            var entity = CurrentWorld.Create(types);

            // Make sure that classes have an instance instead of null.
            InitializeComponentDefaults(entity, types);

            AddIdentifier(entity);

            MapBlueprint(entity, blueprint);

            ProcessBlueprints(entity, blueprint);
            ProcessComponents(entity);

            EntityEvents.EntityCollectionChanged?.Invoke();
            return entity;
        }

        public Entity CreateEmptyEntity()
        {
            return CurrentWorld.Create();
        }

        public void CreateWorld()
        {
            World.Create();
        }

        public void Despawn(Entity entity)
        {
            if (Actors.TryGetValue(entity, out var actor))
            {
                _objectManager?.Unmake(actor?.gameObject);
                Actors.Remove(entity);
            }

            if (entity.Has<Spawned>())
            {
                entity.Remove<Spawned>();
            }
        }

        public void Destroy(Entity entity)
        {
            Despawn(entity);
            CurrentWorld.Destroy(entity);

            EntityEvents.EntityCollectionChanged?.Invoke();
        }

        public void DestroyAllEntities()
        {
            GetAllEntities().ForEach(Destroy);
            Actors.Clear();
        }

        public void DestroyWorld(World world)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (world != null)
            {
                World.Destroy(world);
                EntityEvents.EntityCollectionChanged?.Invoke();
            }
        }

        public void DestroyWorlds()
        {
            foreach (var world in World.Worlds)
            {
                DestroyWorld(world);
            }
        }

        public List<Entity> GetAllEntities()
        {
            var query = new QueryDescription();
            var list = new List<Entity>();

            CurrentWorld?.GetEntities(in query, list);

            return list;
        }

        public void Restore(Entity entity, EntityData data)
        {
            foreach (var component in data.components)
            {
                if (entity.Has(typeof(object)))
                {
                    entity.Set(component);
                }
                else
                {
                    entity.Add(component);
                }
            }

            if (entity.Has<BlueprintReference>())
            {
                var reference = entity.Get<BlueprintReference>();
                if (!string.IsNullOrEmpty(reference.blueprintId) &&
                    _blueprintManager.Blueprints.TryGetValue(reference.blueprintId, out var blueprint))
                {
                    reference.blueprint = blueprint;
                    entity.Set(reference);
                }
                else
                {
                    Debug.LogError($"Blueprint {reference.blueprintId} not found: {entity}");
                }
            }
        }

        public GameObject Spawn(Entity entity)
        {
            if (!entity.Has<BlueprintReference>())
            {
                Debug.LogError("Entity has no BlueprintReference!");
                return null;
            }

            var reference = entity.Get<BlueprintReference>();
            var blueprint = entity.GetBlueprint();
            if (blueprint == null)
            {
                Debug.LogError($"Entity {entity} has no blueprint!");
                return null;
            }

            RandomizePrefab(entity, blueprint, ref reference);

            var prefab = _resourceManager.GetPrefab(blueprint, reference);
            if (prefab == null)
            {
                Debug.LogError($"Entity {entity} has no prefab!");
                return null;
            }

            var transform = entity.GetOrDefault<STransform>();
            var position = (Vector3)transform.position;
            var rotation = (Vector3)transform.rotation;

            var gameObject = _objectManager.Make(
                prefab,
                position: position,
                rotation: rotation,
                entity: entity
            );

            if (gameObject == null)
            {
                return null;
            }

            if (!entity.Has<Spawned>())
            {
                entity.Add<Spawned>();
            }

            Actors[entity] = gameObject.GetComponent<Actor>();
            return gameObject;
        }

        private void AddIdentifier(Entity entity)
        {
            var id = Identity++;
            var identifier = new Identifier
            {
                Id = id
            };

            entity.Ensure(identifier);
        }

        private Archetype EnsureArchetype(Type archetypeType)
        {
            if (!_archetypes.TryGetValue(archetypeType, out Archetype archetype))
            {
                var type = typeof(Archetype);

                archetype = (Archetype)Activator.CreateInstance(archetypeType);
                _archetypes[type] = archetype;
            }

            return archetype;
        }

        private void InitializeComponentDefaults(Entity entity, ComponentType[] types)
        {
            var classTypes = types
                .Where(t => t.Type.IsClass)
                .ToList();

            foreach (var componentType in classTypes)
            {
                var instance = Activator.CreateInstance(componentType.Type);
                entity.Set(instance);
            }
        }

        private void MapBlueprint(Entity entity, Blueprint blueprint)
        {
            if (blueprint == null)
            {
                return;
            }

            entity.Ensure(new BlueprintReference()
            {
                blueprintId = blueprint.id,
                blueprint = blueprint
            });
        }

        private void ProcessBlueprints(Entity entity, Blueprint blueprint)
        {
            foreach (var processor in _blueprintProcessors)
            {
                if (processor.CanProcess(blueprint))
                {
                    processor.Process(entity, blueprint);
                }
            }
        }

        private void ProcessComponents(Entity entity)
        {
            foreach (var processor in _componentProcessors)
            {
                if (processor.CanProcess(entity))
                {
                    processor.Process(entity);
                }
            }
        }

        private void RandomizePrefab(
            Entity entity,
            Blueprint blueprint,
            ref BlueprintReference reference
        )
        {
            if (!reference.prefabIndex.isSet)
            {
                reference.prefabIndex = UnityEngine.Random.Range(0, blueprint.prefabs.Length);
                entity.Set(reference);
            }
        }
    }
}