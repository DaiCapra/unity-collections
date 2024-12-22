using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.Core.Utils;
using DataForge.Blueprints;
using DataForge.Data;
using DataForge.Processors;

namespace DataForge.Entities
{
    public class EntityManager
    {
        private readonly Dictionary<Type, Archetype> _archetypes = new();
        private readonly List<IBlueprintProcessor> _blueprintProcessors = new();
        private readonly List<IComponentProcessor> _componentProcessors = new();

        public EntityManager()
        {
            AddComponentProcessor(new STransformProcessor());
        }

        public Dictionary<Entity, ActorContext> ActorContexts { get; set; } = new();

        public ulong Identity { get; set; }
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

            return entity;
        }

        public void CreateWorld()
        {
            World.Create();
        }

        public void Despawn(Entity entity)
        {
        }

        public void Destroy(Entity entity)
        {
            CurrentWorld.Destroy(entity);
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
        }

        public void Spawn(Entity entity)
        {
        }

        private void AddIdentifier(Entity entity)
        {
            var id = Identity++;
            var identifier = new Identifier
            {
                Id = id
            };

            entity.Add(identifier);
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
            if (blueprint == null || !entity.Has<BlueprintReference>())
            {
                return;
            }

            entity.Set(new BlueprintReference()
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
    }
}