using System;
using System.Collections.Generic;

namespace DataForge.Editor
{
    public class ObjectData
    {
        public string name;
        public Type type;
        public Dictionary<string, object> map = new();
        public List<ObjectData> children = new();
        public bool HasChildren => children.Count > 0;
    }
}