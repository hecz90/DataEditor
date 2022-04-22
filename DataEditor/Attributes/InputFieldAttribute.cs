using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class InputFieldAttribute : FieldAttribute
    {
        
        public InputType InputType { get; private set; }
        public InputFieldAttribute(string name, int index, InputType inputType) : base(name, index, FieldType.Input)
        {
            InputType = inputType;
        }
    }
}