using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataForge.Editor.Reflection
{
    public class TypeCache
    {
        public Dictionary<Type, List<FieldInfo>> fieldMap;
        public Dictionary<Type, List<PropertyInfo>> propertyMap;

        public TypeCache()
        {
            fieldMap = new();
            propertyMap = new();
        }
    }
}