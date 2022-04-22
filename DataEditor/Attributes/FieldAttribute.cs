using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public abstract class FieldAttribute : Attribute
    {
        public string Name { get; private set; }
        public FieldType FieldType { get; private set; }

        public int Index { get; private set; }

        public bool AutoApply;

        public FieldAttribute(string name, int index, FieldType fieldType)
        {
            Name = name;
            Index = index;
            FieldType = fieldType;
        }
    }
}