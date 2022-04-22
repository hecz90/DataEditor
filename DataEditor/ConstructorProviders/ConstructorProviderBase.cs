using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DataEditor.Attributes;

namespace DataEditor.ConstructorProviders
{
    public abstract class ConstructorProviderBase<T> : IConstructorProvider where T : MethodBase
    {
        protected T Constructor;

        public List<KeyValuePair<ParameterInfo, FieldAttribute>> Parameters { get; private set; }
        public string Name { get { return Constructor.Name; } }
        
        public ConstructorProviderBase(T constructor)
        {
            Constructor = constructor;

            var parameterInfos = constructor.GetParameters();
            Parameters = new List<KeyValuePair<ParameterInfo, FieldAttribute>>(parameterInfos.Length - 1);
            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var parameterInfo = parameterInfos[i];
                if (i == 0)
                {
                    if (!parameterInfo.ParameterType.IsAssignableFrom(typeof(ICollection)))
                        throw new ArgumentException(string.Format("fist parameter of constructor({0}) must be ICollection", constructor.DeclaringType.Name));

                    continue;
                }

                var fieldAttribute = parameterInfo.GetCustomAttribute<FieldAttribute>();
                if (fieldAttribute == null)
                    throw new ArgumentException(string.Format("second and later parameters of constructor({0}) must have 'FieldAttribute'", constructor.DeclaringType.Name));

                Parameters.Add(new KeyValuePair<ParameterInfo, FieldAttribute>(parameterInfo, fieldAttribute));
            }
        }


        public object Invoke(ICollection collection, IList<object> parameters)
        {
            var p = new object[parameters.Count + 1];
            p[0] = collection;
            for (int i = 0; i < parameters.Count; i++) p[i + 1] = parameters[i];

            return InvokeInternal(p);

        }

        protected abstract object InvokeInternal(object[] parameters);
    }
}