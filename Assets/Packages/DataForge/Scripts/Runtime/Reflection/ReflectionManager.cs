using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataForge.Reflection
{
    public class ReflectionCache
    {
        public Dictionary<string, Member> attributeMembers = new();
        public Dictionary<string, Member> members = new();
    }

    public static class ReflectionManager
    {
        private static readonly Dictionary<Type, ReflectionCache> Map = new();

        public static Dictionary<string, Member> GetMembers(Type type)
        {
            if (Map.TryGetValue(type, out var c))
            {
                return c.attributeMembers;
            }

            var map = new Dictionary<string, Member>();

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = type
                .GetFields(flags);

            foreach (var field in fields)
            {
                map[field.Name] = new FieldMember(field);
            }

            var properties = type
                .GetProperties(flags);

            foreach (var property in properties)
            {
                map[property.Name] = new PropertyMember(property);
            }

            if (!Map.TryGetValue(type, out var cache))
            {
                cache = new();
                Map[type] = cache;
            }

            cache.attributeMembers = map;
            return map;
        }

        public static Dictionary<string, Member> GetMembers<T>(Type type) where T : Attribute
        {
            if (Map.TryGetValue(type, out var c))
            {
                return c.attributeMembers;
            }

            var map = new Dictionary<string, Member>();

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = type
                .GetFields(flags)
                .Where(t => t.GetCustomAttribute<T>() != null);

            foreach (var field in fields)
            {
                map[field.Name] = new FieldMember(field);
            }

            var properties = type
                .GetProperties(flags)
                .Where(t => t.GetCustomAttribute<T>() != null);

            foreach (var property in properties)
            {
                map[property.Name] = new PropertyMember(property);
            }

            if (!Map.TryGetValue(type, out var cache))
            {
                cache = new();
                Map[type] = cache;
            }

            cache.attributeMembers = map;
            return map;
        }
    }
}