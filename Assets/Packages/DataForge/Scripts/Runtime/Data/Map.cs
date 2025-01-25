using System;
using System.Collections.Generic;

namespace DataForge.Data
{
    [Serializable]
    public class Map<T> : Dictionary<ulong, T>
    {
        public ulong identity = 1;

        public void Add(T t)
        {
            ulong id = identity++;
            this[id] = t;
        }
    }
}