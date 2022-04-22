using System;
using DataEditor.Attributes;

namespace DataEditor.Mapping
{
    public class MemberInfo : IComparable<MemberInfo>, IComparable
    {
        public System.Reflection.MemberInfo Info;
        public FieldAttribute FieldAttribute;
        public EntryFieldAttribute EntryFieldAttribute;
        public TabAttribute TabAttribute;

        public int CompareTo(MemberInfo other)
        {
            if (FieldAttribute == null)
            {
                if (other.FieldAttribute == null) return 0;
                return -1;
            }
            if (other.FieldAttribute == null) return 1;
            return FieldAttribute.Index.CompareTo(other.FieldAttribute.Index);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            if (!(obj is MemberInfo)) throw new ArgumentException("Object must be of type MemberData");
            return CompareTo((MemberInfo)obj);
        }
    }
}