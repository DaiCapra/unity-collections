using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using DataForge.Blueprints;
using DataForge.Data;
using UnityEngine;

namespace DataForge.Entities
{
    public static class EntityExtensions
    {
        public delegate void RefAction<T>(ref T value);

        public static bool AreComponentsSame(this Entity entity, Entity other)
        {
            var components = entity.GetAllComponents();
            var otherComponents = other.GetAllComponents();

            bool isEqual = components.SequenceEqual(otherComponents);
            return isEqual;
        }

        public static void Change<T>(this Entity entity, RefAction<T> action)
        {
            var component = entity.Get<T>();
            action?.Invoke(ref component);
            entity.Set(component);
        }

        public static void Ensure<T>(this Entity entity, T t)
        {
            if (entity.Has<T>())
            {
                entity.Set(t);
            }
            else
            {
                entity.Add(t);
            }
        }

        public static Blueprint GetBlueprint(this Entity entity)
        {
            if (!entity.Has<BlueprintReference>())
            {
                return null;
            }

            return entity.Get<BlueprintReference>().blueprint;
        }

        public static string GetName(this Entity entity)
        {
            var id = entity.Get<Identifier>().Id;
            return $"Entity {id}";
        }

        public static string GetBlueprintId(this Entity entity)
        {
            return entity.Has<BlueprintReference>()
                ? entity.Get<BlueprintReference>().blueprintId
                : null;
        }

        public static void SetPosition(this Entity entity, Vector3 position)
        {
            var transform = entity.Get<STransform>();
            transform.position = position;
            entity.Set(transform);
        }

        public static ulong GetId(this Entity entity)
        {
            if (!entity.Has<Identifier>())
            {
                return 0;
            }

            return entity.Get<Identifier>().Id;
        }

        public static T GetOrDefault<T>(this Entity entity)
        {
            return entity.Has<T>()
                ? entity.Get<T>()
                : default;
        }
    }
}