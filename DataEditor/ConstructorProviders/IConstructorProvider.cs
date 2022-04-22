using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DataEditor.Attributes;

namespace DataEditor.ConstructorProviders
{
    public interface IConstructorProvider
    {
        List<KeyValuePair<ParameterInfo, FieldAttribute>> Parameters { get; }

        object Invoke(ICollection collection, IList<object> parameters);

        string Name { get; }
    }
}