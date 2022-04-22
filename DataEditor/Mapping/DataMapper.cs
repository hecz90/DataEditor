using System;
using System.Collections.Generic;
using System.Reflection;
using DataEditor.Attributes;
using DataEditor.ConstructorProviders;

namespace DataEditor.Mapping
{
    public class DataMapper
    {
        public static DataMapper GetMapper(Type type)
        {
            DataMapper mapper;
            if (_mappers.TryGetValue(type, out mapper)) return mapper;

            return _mappers[type] = new DataMapper(type);
        }
        private static readonly Dictionary<Type, DataMapper> _mappers = new Dictionary<Type, DataMapper>();


        public readonly Type Type;
        public readonly List<MemberInfo> Members = new List<MemberInfo>();
        public readonly Dictionary<MethodInfo, ActionAttribute> Actions = new Dictionary<MethodInfo, ActionAttribute>();

        public IConstructorProvider Constructor;

        public System.Reflection.MemberInfo Key;

        public DataMapper(Type type)
        {
            Type = type;

            foreach (var memberInfo in type.GetMembers(BindingFlags.Public|BindingFlags.NonPublic| BindingFlags.Instance))
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                    case MemberTypes.Property:
                        TryAddMemberData(memberInfo);
                        TryFillKey(memberInfo);
                        break;
                    case MemberTypes.Constructor:
                        TryFillConstructor(memberInfo);
                        break;
                }
            }

            Members.Sort();

            foreach (var memberInfo in type.GetMembers(BindingFlags.Public | BindingFlags.Static))
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Method:
                        TryAdd((MethodInfo)memberInfo, Actions);
                        TryFillConstructor(memberInfo);
                        break;
                }
            }
        }

        private void TryAddMemberData(System.Reflection.MemberInfo memberInfo)
        {
            var field = memberInfo.GetCustomAttribute<FieldAttribute>();
            var entry = memberInfo.GetCustomAttribute<EntryFieldAttribute>();
            if (field == null && entry == null) return;

            Members.Add(new MemberInfo
            {
                Info = memberInfo,
                FieldAttribute = field,
                EntryFieldAttribute = entry,
                TabAttribute = memberInfo.GetCustomAttribute<TabAttribute>(),
            });

        }

        private void TryFillConstructor(System.Reflection.MemberInfo memberInfo)
        {
            var attribute = memberInfo.GetCustomAttribute<ConstructorAttribute>();
            if (attribute == null) return;

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Constructor:
                    Constructor = new ConstructorProvider((ConstructorInfo)memberInfo);
                    break;
                case MemberTypes.Method:
                    Constructor = new StaticFactoryProvider((MethodInfo)memberInfo);
                    break;
            }
        }


        private void TryFillKey(System.Reflection.MemberInfo memberInfo)
        {
            var attribute = memberInfo.GetCustomAttribute<KeyAttribute>();
            if (attribute == null) return;

            Key = memberInfo;
        }


        private static void TryAdd<TMemberInfo, TAttribute>(TMemberInfo memberInfo, Dictionary<TMemberInfo, TAttribute> dic) 
            where TMemberInfo : MethodInfo
            where TAttribute : Attribute
        {
            var attribute = memberInfo.GetCustomAttribute<TAttribute>();
            if (attribute != null) dic[memberInfo] = attribute;
        }
    }
}