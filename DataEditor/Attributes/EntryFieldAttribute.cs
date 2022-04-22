using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EntryFieldAttribute : Attribute
    {
        public string Name;

        public EntryFieldAttribute()
        {
            Name = string.Empty;
        }

        public EntryFieldAttribute(string name)
        {
            Name = name;
        }
    }
}