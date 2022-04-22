using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CollectionFieldAttribute : FieldAttribute
    {
        public Type TargetType { get; private set; }

        public ListOptions Options = ListOptions.SimpleEntry;

        public CollectionFieldAttribute(string name, int index, Type targetType) : base(name, index, FieldType.Collection)
        {
            TargetType = targetType;
        }
    }
}