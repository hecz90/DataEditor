using System;

namespace DataEditor.Attributes
{
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DropdownFieldAttribute : FieldAttribute
    {
        public Type DataSourceType;

        public DropdownFieldAttribute(string name, int index, Type dataSourceType = null) : base(name, index, FieldType.Dropdown)
        {
            DataSourceType = dataSourceType;
        }
    }
}