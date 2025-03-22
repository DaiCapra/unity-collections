using System;
using System.Collections.Generic;
using Arch.Core.Utils;
using DataForge.Data;
using DataForge.Objects;

namespace DataForge.Entities
{
    public abstract class Archetype
    {
        public readonly List<ComponentType> types = new();
        public string Name => GetType().Name.Replace("Archetype", "");

        public Archetype()
        {
            Add<Identifier>();
            Add<BlueprintReference>();
            Add<LayerRef>();
        }

        public ComponentType[] GetComponentTypes()
        {
            return types.ToArray();
        }

        protected void Add<T>()
        {
            Add(typeof(T));
        }

        private void Add(Type type)
        {
            var componentType = Component.GetComponentType(type);
            types.Add(componentType);
        }
    }
}