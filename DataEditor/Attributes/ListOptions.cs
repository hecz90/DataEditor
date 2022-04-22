using System;

namespace DataEditor.Attributes
{
    [Flags]
    public enum ListOptions
    {
        None,
        EditableEntry = 1,
        ApplyButton = 2,
        AddButton = 4,
        RemoveButton = 8,
        EditButton = 16,

        ComplexEntry = AddButton | RemoveButton | EditButton,
        SimpleEntry = EditableEntry | AddButton  | RemoveButton,
    }
}