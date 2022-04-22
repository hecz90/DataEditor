using System.Collections.Generic;
using System.Linq;
using DataEditor.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class ImageFieldView : FieldView<string>
{
    public Dropdown Dropdown;
    public RawImage RawImage;
    public List<DropdownOption<string>> Options = new List<DropdownOption<string>>();

    public override FieldType FieldType
    {
        get { return FieldType.Image; }
    }

    public override string Value
    {
        get { return _value; }
        set
        {
            _value = value;

            RawImage.texture = Resources.Load<Texture>(GetRootPath() + "/" + _value);

            UpdateDropdown();
        }
    }
    private string _value;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Dropdown) Dropdown.onValueChanged.RemoveListener(OnFieldChanged);
    }


    public override void SetObject(object owner)
    {
        if (Dropdown)
        {
            Dropdown.onValueChanged.RemoveListener(OnFieldChanged);
            Dropdown.ClearOptions();
        }


        base.SetObject(owner);
        if (Dropdown)
        {
            FillOptions(owner);
            Dropdown.AddOptions(Options.Select(n => n.Name).ToList());
            UpdateDropdown();
            Dropdown.onValueChanged.AddListener(OnFieldChanged);
        }
    }

    private void FillOptions(object owner)
    {
        var resources = Resources.LoadAll<Texture>(GetRootPath());

        Options = resources.Select(i => new DropdownOption<string>
        {
            Name = i.name,
            Value =i.name
        }).ToList();
    }

    private string GetRootPath()
    {
        return ((ImageFieldAttribute) Context.FieldAttribute).RootPath;
    }

    private void UpdateDropdown()
    {
        if (!Dropdown) return;

        var index = Options.FindIndex(o => o.Value.Equals(_value));
        if (Dropdown.value == index) return;
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
        Value = Options[Dropdown.value].Value;
        UpdateChanged();
    }

}