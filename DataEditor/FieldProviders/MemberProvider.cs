using System.Reflection;

namespace DataEditor.FieldProviders
{
    public abstract class MemberProvider<T>
    {
        public abstract T GetValue();
        public abstract void SetValue(T value);

        public static MemberProvider<object> Create(object owner, MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return new FieldProvider<object>(owner, memberInfo as FieldInfo);
                case MemberTypes.Property:
                    return new PropertyProvider<object>(owner, memberInfo as PropertyInfo);
                default:
                    return null;
            }
        }
    }

    public class FieldProvider<T> : MemberProvider<T>
    {
        private readonly object _obj;
        private readonly FieldInfo _fieldInfo;

        public FieldProvider(object obj, FieldInfo fieldInfo)
        {
            _obj = obj;
            _fieldInfo = fieldInfo;
        }
        public override T GetValue()
        {
            return (T)_fieldInfo.GetValue(_obj);
        }

        public override void SetValue(T value)
        {
            _fieldInfo.SetValue(_obj, value);
        }
    }

    public class PropertyProvider<T> : MemberProvider<T>
    {
        private readonly object _obj;
        private readonly PropertyInfo _propertyInfo;

        public PropertyProvider(object obj, PropertyInfo propertyInfo)
        {
            _obj = obj;
            _propertyInfo = propertyInfo;
        }
        public override T GetValue()
        {
            return (T)_propertyInfo.GetValue(_obj, null);
        }

        public override void SetValue(T value)
        {
            _propertyInfo.SetValue(_obj, value, null);
        }
    }
}