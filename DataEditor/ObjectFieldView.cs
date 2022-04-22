using System.Collections.Generic;
using System.Linq;
using DataEditor.Attributes;
using UnityEngine;

public interface IObjectView
{
    Transform Content { get; }

    bool AutoApply { get; set; }

    void AddChild(View view);
}

public class ObjectFieldView : FieldView<object>, IObjectView
{
    public Transform Content;
    
    public override FieldType FieldType
    {
        get { return FieldType.Object; }
    }

    public void AddChild(View view)
    {
        _items.Add(view);
    }

    public override object Value { get; set; }


    private readonly List<View> _items = new List<View>();
    
    public void Clear()
    {
        foreach (var fieldView in _items)
        {
            fieldView.Changed -= UpdateChanged;
        }
        _items.Clear();
    }
    
    public override void SetObject(object owner)
    {
        foreach (var fieldView in _items)
        {
            fieldView.Changed -= UpdateChanged;
            fieldView.Changed += UpdateChanged;
        }
        base.SetObject(owner);

        foreach (var fieldView in _items) fieldView.SetObject(Value);

        UpdateChanged();
    }

    public override void Apply()
    {
        foreach (var fieldView in _items) fieldView.Apply();
        base.Apply();

        UpdateChanged();
    }

    protected override void UpdateChanged()
    {
        IsChanged = _items.Any(f => f.IsChanged);
    }

    Transform IObjectView.Content { get { return Content; } }
    bool IObjectView.AutoApply { get { return AutoApply; } set { AutoApply = value; } }
}
