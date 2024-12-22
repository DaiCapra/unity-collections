using System;
using System.Reflection;

namespace DataForge.Editor.Reflection
{
    public class PropertyMember : Member
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyMember(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public override object GetValue(object source)
        {
            try
            {
                return _propertyInfo.GetValue(source)!;
            }
            catch (Exception e)
            {
                return default;
            }
        }

        public override T GetCustomAttribute<T>()
        {
            return _propertyInfo.GetCustomAttribute<T>()!;
        }

        public override string Name()
        {
            return _propertyInfo.Name;
        }

        public override void SetValue(object source, object value)
        {
            _propertyInfo.SetValue(source, value);
        }

        public override Type Type()
        {
            return _propertyInfo.PropertyType;
        }
    }
}