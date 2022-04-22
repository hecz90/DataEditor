using System.Reflection;
using DataEditor.Mapping;

namespace DataEditor.ConstructorProviders
{
    public class StaticFactoryProvider : ConstructorProviderBase<MethodInfo>
    {
        public StaticFactoryProvider(MethodInfo constructor) : base(constructor) { }

        protected override object InvokeInternal(object[] parameters)
        {
            return Constructor.Invoke(null, parameters);
        }
    }
}