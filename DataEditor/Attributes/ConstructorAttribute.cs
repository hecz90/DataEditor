using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    public class ConstructorAttribute : Attribute
    {
        public ConstructorAttribute()
        {
        }
    }
}