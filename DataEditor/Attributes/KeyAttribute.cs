using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute()
        {
        }
    }
}