using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DataEditor.Attributes;
using UnityEngine.UI;

[Serializable]
public class DropdownOption<T>
{
    public T Value;
    public string Name;
}

public interface IDropdownDataSource
{
    List<DropdownOption<object>> GetOptions(object owner, object currentValue);
}

public class EnumDropdownDataSource : IDropdownDataSource
{
    public List<DropdownOption<object>> GetOptions(object owner, object currentValue)
    {
        var enumType = currentValue.GetType();
        return Enum.GetValues(enumType).Cast<Enum>().Select(e => new DropdownOption<object>
        {
            Value = e,
            Name = e.GetEnumDescriptionName(enumType)
        }).ToList();
    }
}

public class BoolDropdownDataSource : IDropdownDataSource
{
    public List<DropdownOption<object>> GetOptions(object owner, object currentValue)
    {
        var result = new List<DropdownOption<object>>(2);
        result.Add(new DropdownOption<object>
        {
            Name = "Нет",
            Value = false
        });
        result.Add(new DropdownOption<object>
        {
            Name = "Да",
            Value = true
        });
        return result;
    }
}



public class DropdownFieldView : FieldView<object>
{
    public Dropdown Dropdown;
    public List<DropdownOption<object>> Options = new List<DropdownOption<object>>();

    public override FieldType FieldType
    {
        get { return FieldType.Dropdown; }
    }

    public override object Value
    {
        get { return _value; }
        set
        {
            _value = value;

            UpdateDropdown();
        }
    }
    private object _value;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Dropdown.onValueChanged.RemoveListener(OnFieldChanged);
    }


    public override void SetObject(object owner)
    {
        Dropdown.onValueChanged.RemoveListener(OnFieldChanged);

        Dropdown.ClearOptions();

        base.SetObject(owner);

        FillOptions(owner);
        Dropdown.AddOptions(Options.Select(n => n.Name).ToList());
        UpdateDropdown();
        Dropdown.onValueChanged.AddListener(OnFieldChanged);
    }

    private void FillOptions(object owner)
    {
        var currentValue = Value;

        var dataSourceType = ((DropdownFieldAttribute) Context.FieldAttribute).DataSourceType;
        if (dataSourceType == null && currentValue is Enum) dataSourceType = typeof(EnumDropdownDataSource);

        Options = ((IDropdownDataSource) Activator.CreateInstance(dataSourceType)).GetOptions(owner, Value);
    }

    private void UpdateDropdown()
    {
        var index = Options.FindIndex(o => Equals(o.Value, _value));
        if (index < 0)
        {
            if (Options.Count == 0) return;

            index = 0;
            _value = Options[0].Value;
            UpdateChanged();
        }

        Dropdown.value = index;
    }

    private void OnFieldChanged(int arg)
    {
        _value = Options[Dropdown.value].Value;
        UpdateChanged();
    }

}