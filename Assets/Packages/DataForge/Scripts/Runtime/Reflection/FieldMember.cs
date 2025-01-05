using System;
using System.Reflection;

namespace DataForge.Reflection
{
    public class FieldMember : Member
    {
        private readonly FieldInfo _fieldInfo;

        public FieldMember(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public override object GetValue(object source)
        {
            return _fieldInfo.GetValue(source)!;
        }

        public override T GetCustomAttribute<T>()
        {
            return _fieldInfo.GetCustomAttribute<T>()!;
        }

        public override string Name()
        {
            return _fieldInfo.Name;
        }

        public override void SetValue(object source, object value)
        {
            _fieldInfo.SetValue(source, value);
        }

        public override Type Type()
        {
            return _fieldInfo.FieldType;
        }

        public bool Equals(FieldMember other)
        {
            if (other?._fieldInfo == null)
            {
                return false;
            }

            if (other._fieldInfo != _fieldInfo)
            {
                return false;
            }


            return true;
        }
    }
}