using System;
using System.Collections.Generic;
using System.Linq;

namespace DataForge.Editor
{
    public class ObservableList<T> : List<T>
    {
        public Action<T> ItemAdded { get; set; }
        public Action<T> ItemRemoved { get; set; }
        public Action ItemsCleared { get; set; }
        public Action<IEnumerable<T>> ItemsAdded { get; set; }
        public Action ItemsChanged { get; set; }

        public new void Add(T t)
        {
            base.Add(t);
            ItemAdded?.Invoke(t);
            ItemsChanged?.Invoke();
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            var a = collection as T[] ?? collection.ToArray();
            base.AddRange(a);
            ItemsAdded?.Invoke(a);
            ItemsChanged?.Invoke();
        }

        public new void Clear()
        {
            base.Clear();
            ItemsCleared?.Invoke();
            ItemsChanged?.Invoke();
        }

        public new void Remove(T t)
        {
            base.Remove(t);
            ItemRemoved?.Invoke(t);
            ItemsChanged?.Invoke();
        }
    }
}