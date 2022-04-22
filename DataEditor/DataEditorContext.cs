using System;
using DataEditor.Attributes;
using DataEditor.Mapping;

public class DataEditorContext
{
    public DataEditorController Controller;
    public FieldAttribute FieldAttribute;

    public DataMapper GetEntryMapper()
    {
        return DataMapper.GetMapper(CollectionTargetType());
    }

    public Type CollectionTargetType()
    {
        return ((CollectionFieldAttribute) FieldAttribute).TargetType;
    }
}