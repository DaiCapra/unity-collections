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
    public class EntityManager
    {
        private readonly Dictionary<Type, Archetype> _archetypes = new();
        private readonly List<IBlueprintProcessor> _blueprintProcessors = new();
        private readonly IBlueprintProvider _blueprintProvider;
        private readonly List<IComponentProcessor> _componentProcessors = new();
        private readonly IObjectManager _objectManager;
        private readonly IResourceProvider _resourceProvider;

        public EntityManager(
            IResourceProvider resourceProvider,
            IBlueprintProvider blueprintProvider,
            IObjectManager objectManager
        )
        {
            _blueprintProvider = blueprintProvider;
            _resourceProvider = resourceProvider;
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

        public List<object> Backup(Entity entity)
        {
            var components = entity.GetAllComponents()
                .Where(t => t is not ITransientData)
                .ToList();

            return components;
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

        public void CreateWorld()
        {
            World.Create();
        }

        public void Despawn(Entity entity)
        {
            if (Actors.TryGetValue(entity, out var actor))
            {
                _objectManager.Unmake(actor.gameObject);
                Actors.Remove(entity);
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

        public void Restore(Entity entity, List<object> components)
        {
            foreach (var component in components)
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
                    _blueprintProvider.Blueprints.TryGetValue(reference.blueprintId, out var blueprint))
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

        public void Spawn(Entity entity)
        {
            if (!entity.Has<BlueprintReference>())
            {
                Debug.LogError("Entity has no BlueprintReference!");
                return;
            }

            var reference = entity.Get<BlueprintReference>();
            var blueprint = entity.GetBlueprint();
            if (blueprint == null)
            {
                Debug.LogError($"Entity {entity} has no blueprint!");
                return;
            }

            RandomizePrefab(entity, blueprint, ref reference);

            var prefab = _resourceProvider.GetPrefab(blueprint, reference);
            if (prefab == null)
            {
                Debug.LogError($"Entity {entity} has no prefab!");
                return;
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
                return;
            }

            Actors[entity] = gameObject.GetComponent<Actor>();
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

        public Entity CreateEmptyEntity()
        {
            return CurrentWorld.Create();
        }
    }
}