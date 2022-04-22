using System;
using System.Collections.Generic;

namespace DataEditor.Attributes
{
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ImageFieldAttribute : FieldAttribute
    {
        public string RootPath;

        public ImageFieldAttribute(string name, int index, string rootPath = "") : base(name, index, FieldType.Image)
        {
            RootPath = rootPath;
        }
    }
}