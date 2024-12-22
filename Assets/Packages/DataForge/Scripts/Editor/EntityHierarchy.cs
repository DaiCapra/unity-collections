using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core;
using Arch.Core.Extensions;
using DataForge.Entities;

namespace DataForge.Editor
{
    public class EntityHierarchy : IDisposable
    {
        public EntityHierarchy()
        {
            EntityEvents.EntityCollectionChanged += OnEntityCollectionChanged;
        }

        public ObservableList<Entity> Entities { get; } = new();

        public void Dispose()
        {
            EntityEvents.EntityCollectionChanged -= OnEntityCollectionChanged;
        }

        public void Clear()
        {
            Entities.Clear();
        }
        
        private void OnEntityCollectionChanged()
        {
            var world = World.Worlds.FirstOrDefault();
            if (world == null)
            {
                return;
            }

            Clear();

            var query = new QueryDescription();
            var list = new List<Entity>();
            world.GetEntities(in query, list);

            Entities.AddRange(list);
        }
    }
}