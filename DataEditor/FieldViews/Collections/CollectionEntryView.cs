using System;
using System.Collections.Generic;
using System.Linq;
using DataEditor.Attributes;
using DataEditor.Mapping;
using UnityEngine;
using UnityEngine.UI;

public class CollectionEntryView : ObjectView
{
    public Button ApplyButton;
    public Button EditButton;
    public Button RemoveButton;

    public List<FieldView> Templates;

    public RectTransform EmptySpacePrefab;

    public event Action<CollectionEntryView> Edit;
    public event Action<CollectionEntryView> Remove;

    public void RemoveClicked()
    {
        if (Remove != null) Remove(this);
    }

    public void EditClicked()
    {
        if (Edit != null) Edit(this);
    }

    protected override void UpdateChanged()
    {
        base.UpdateChanged();

        ApplyButton.interactable = IsChanged;
    }

    public void CreateEntryFields( object entry, CollectionFieldAttribute collectionFieldAttribute)
    {
        var mapper = DataMapper.GetMapper(entry.GetType());

        ApplyButton.gameObject.SetActive(collectionFieldAttribute.Options.HasFlag(ListOptions.ApplyButton));
        EditButton.gameObject.SetActive(collectionFieldAttribute.Options.HasFlag(ListOptions.EditButton));
        RemoveButton.gameObject.SetActive(collectionFieldAttribute.Options.HasFlag(ListOptions.RemoveButton));
        var editableEntry = collectionFieldAttribute.Options.HasFlag(ListOptions.EditableEntry);
        foreach (var data in mapper.Members)
        {
            var memberInfo = data.Info;
            var entryAttribute = data.EntryFieldAttribute;
            if (entryAttribute == null) continue;

            var fieldAttribute = data.FieldAttribute ?? new LabelFieldAttribute(entryAttribute.Name, 0);
            if (!editableEntry && (fieldAttribute.FieldType != FieldType.Label && fieldAttribute.FieldType != FieldType.Image))
                fieldAttribute = new LabelFieldAttribute(fieldAttribute.Name, fieldAttribute.Index);

            var fieldType = fieldAttribute.FieldType;

            var fieldTemplate = Templates.Find(t => t.FieldType == fieldType);
            if (fieldTemplate == null) fieldTemplate = Templates.Find(t => t.FieldType == FieldType.Label);

            var fieldView = Instantiate(fieldTemplate, Content);

            fieldView.FieldName = memberInfo.Name;
            fieldView.name = fieldView.name = fieldTemplate.name + " [" + memberInfo.Name + "]";
            fieldView.SetName(entryAttribute.Name);
            fieldView.Context = new DataEditorContext { Controller = Context.Controller, FieldAttribute = fieldAttribute };
            fieldView.AutoApply = fieldAttribute.AutoApply;

            Items.Add(fieldView);
        }
    }

    public IEnumerable<FieldAttribute> Fields { get { return Items.Select(i => ((FieldView)i).Context.FieldAttribute); } }
    
    public void Resize(List<string> titles)
    {
        var itemsByTitles = Items.ToDictionary(i => ((FieldView) i).Context.FieldAttribute.Name, f => f.transform);

        foreach (Transform child in Content)
        {
            if (!itemsByTitles.ContainsValue(child)) Destroy(child.gameObject);
        }

        var preferredWidth = Content.sizeDelta.x/ titles.Count;
        
        for (int i = 0; i < titles.Count; i++)
        {
            Transform tr;
            if (!itemsByTitles.TryGetValue(titles[i], out tr)) tr = Instantiate(EmptySpacePrefab, Content);

            tr.SetSiblingIndex(i);
            var layoutElement = tr.gameObject.GetComponent<LayoutElement>() ?? tr.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = preferredWidth;
        }
    }
}


