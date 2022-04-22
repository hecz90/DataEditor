using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectView : View, IObjectView
{
    public RectTransform Content;

    protected readonly List<View> Items = new List<View>();

    private object _owner;

    public override void SetObject(object owner)
    {
        _owner = owner;

        foreach (var fieldView in Items)
        {
            fieldView.Changed -= UpdateChanged;
            fieldView.Changed += UpdateChanged;
            fieldView.SetObject(owner);
        }

        UpdateChanged();
    }

    public override void Apply()
    {
        foreach (var fieldView in Items) fieldView.Apply();

        SetObject(_owner);
    }

    protected virtual void UpdateChanged()
    {
        IsChanged = Items.Any(f => f.IsChanged);

    }

    public void AddChild(View view)
    {
        Items.Add(view);
    }

    public void Clear()
    {
        foreach (var field in Items)
        {
            field.Changed -= UpdateChanged;
            Destroy(field.gameObject);
        }
        Items.Clear();
    }

    Transform IObjectView.Content { get{ return Content; } }
    bool IObjectView.AutoApply { get { return AutoApply; } set { AutoApply = value; } }

    public DataEditorContext Context { get; set; }
}
