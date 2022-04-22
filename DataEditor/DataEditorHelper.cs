using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public static class DataEditorHelper
{
    public static string GetEnumDescriptionName(this object obj, Type enumType)
    {
        var memberInfos = enumType.GetMember(obj.ToString());
        var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
        var descriptionAttribute = enumValueMemberInfo.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute != null ? descriptionAttribute.Description : obj.ToString();
    }

    public static string GetEnumDescriptionName(this object obj)
    {
        return GetEnumDescriptionName(obj, obj.GetType());
    }
}