using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LabelFieldAttribute : FieldAttribute
    {
        public LabelFieldAttribute(string name, int index) : base(name, index, FieldType.Label)  { }
    }
}