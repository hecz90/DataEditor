using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ObjectFieldAttribute : FieldAttribute
    {
        public ObjectFieldAttribute(string name, int index) : base(name, index, FieldType.Object)
        {
        }
    }
}