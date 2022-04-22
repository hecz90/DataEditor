using System;

namespace DataEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public string Name { get; private set; }

        public ActionAttribute(string name)
        {
            Name = name;
        }
    }
}