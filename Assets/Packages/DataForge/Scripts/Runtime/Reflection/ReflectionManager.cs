using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataForge.Reflection
{
    public static class ReflectionManager
    {
        private static readonly Dictionary<Type, Dictionary<string, Member>> MemberMap = new();

        public static Dictionary<string, Member> GetMembers<T>(Type type) where T : Attribute
        {
            if (MemberMap.TryGetValue(type, out var members))
            {
                return members;
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

            MemberMap[type] = map;
            return map;
        }
    }
}