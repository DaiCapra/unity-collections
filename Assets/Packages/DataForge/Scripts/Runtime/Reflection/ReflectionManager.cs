using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataForge.Reflection
{
    public static class ReflectionManager
    {
        private static readonly Dictionary<Type, Dictionary<string, Member>> MemberMap = new();

        public static Member GetMember(Type type, string name)
        {
            var members = GetMembers(type);
            return members.TryGetValue(name, out var member)
                ? member
                : null;
        }

        public static Dictionary<string, Member> GetMembers(Type type)
        {
            if (MemberMap.TryGetValue(type, out var members))
            {
                return members;
            }

            var map = new Dictionary<string, Member>();

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = type.GetFields(flags);

            foreach (var field in fields)
            {
                map[field.Name] = new FieldMember(field);
            }

            var properties = type.GetProperties(flags);
            foreach (var property in properties)
            {
                map[property.Name] = new PropertyMember(property);
            }

            MemberMap[type] = map;
            return map;
        }
    }
}