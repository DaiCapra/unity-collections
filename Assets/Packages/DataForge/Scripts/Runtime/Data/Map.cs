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
            if (t is IId i)
            {
                i.Id = id;
            }

            this[id] = t;
        }
    }
}