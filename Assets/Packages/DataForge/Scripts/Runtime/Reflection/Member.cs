using System;
using System.Reflection;

namespace DataForge.Reflection
{
    public abstract class Member
    {
        public static Member Create(object member)
        {
            return (member switch
            {
                FieldInfo fi => new FieldMember(fi),
                PropertyInfo pi => new PropertyMember(pi),
                _ => null
            })!;
        }

        public abstract object GetValue(object source);

        public abstract T GetCustomAttribute<T>() where T : Attribute;

        public abstract string Name();

        public abstract void SetValue(object source, object value);

        public abstract Type Type();
    }
}