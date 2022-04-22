using System.Reflection;
using DataEditor.Mapping;

namespace DataEditor.ConstructorProviders
{
    public class ConstructorProvider : ConstructorProviderBase<ConstructorInfo>
    {
        public ConstructorProvider(ConstructorInfo constructor) : base(constructor) { }

        protected override object InvokeInternal( object[] parameters)
        {
            return Constructor.Invoke(parameters);
        }
    }
}