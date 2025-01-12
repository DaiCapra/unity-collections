using System.Collections.Generic;

namespace DataForge.Reflection
{
    public class ReflectionCache
    {
        public Dictionary<string, Member> attributeMembers = new();
        public Dictionary<string, Member> members = new();
    }
}