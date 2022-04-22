using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataEditor.Attributes;
using DataEditor.FieldProviders;
using UnityEngine;
using UnityEngine.UI;

public class CollectionFieldView : FieldView<ICollection>
{
    public RectTransform TitlesContent;
    public RectTransform Content;
    

    public Button AddButton;
    public CollectionEntryView EntryViewPrefab;

    public Text TitleViewPrefab;

    public ConstructorParametersView ConstructorParametersView;

    public override FieldType FieldType {  get { return FieldType.Collection; } }
    
    private readonly List<CollectionEntryView> _entryViews = new List<CollectionEntryView>();
    private readonly List<object> _entries = new List<object>();

    private Type _collectionType;
    private bool _isDictionary;

    private IEnumerator _updateGridCoroutine;
    private bool _titlesDirty = true;
    private List<string> _titleTexts = new List<string>();
    private List<Text> _titles = new List<Text>();

    public override ICollection Value
    {
        get
        {
            if (_collectionType == null) return null;
            if (_isDictionary)
            {
                var result = (IDictionary) Activator.CreateInstance(_collectionType, _entries.Count);
                foreach (object entry in _entries)
                {
                    result[MemberProvider<object>.Create(entry, Context.GetEntryMapper().Key).GetValue()] = entry;
                }

                return result;
            }
            else 
            {
                var result = (IList)Activator.CreateInstance(_collectionType, _entries.Count);
                foreach (object entry in _entries)
                {
                    result.Add(entry);
                }

                return result;
            }
        }
        set
        {
            Clear();

            _collectionType = value.GetType();
            _isDictionary = value is IDictionary;

            _entries.Capacity = Math.Max(_entries.Capacity, value.Count);
            _entryViews.Capacity = Math.Max(_entryViews.Capacity, value.Count);
            
            foreach (object t in _isDictionary ? ((IDictionary)value).Values : value)
            {
                AddEntry(t);
            }

            ConstructorParametersView.CreateParameterViews(Context);
        }
    }
    
    protected override void OnDisable()
    {
        if (_updateGridCoroutine != null) _updateGridCoroutine = null;
    }

    protected override void OnEnable()
    {
        _titlesDirty = true;
        UpdateGrid();
    }

    void Update()
    {
        UpdateTitlesSize();
    }

    public override void SetObject(object owner)
    {
        base.SetObject(owner);

        AddButton.gameObject.SetActive(((CollectionFieldAttribute)Context.FieldAttribute).Options.HasFlag(ListOptions.AddButton));
    }

    public override void Apply()
    {
        foreach (var entryView in _entryViews) entryView.Apply();
        base.Apply();
    }


    protected override void UpdateChanged()
    {
        IsChanged = IsChanged || _entryViews.Any(f => f.IsChanged);
    }

    private void Clear()
    {
        foreach (var entryView in _entryViews) CleanEntryView(entryView);
        _entryViews.Clear();
        _entries.Clear();
        _collectionType = null;
    }

    private void AddEntry(object entry)
    {
        var entryView = Instantiate(EntryViewPrefab, Content);
        InitEntryView(entryView);
        CreateEntryFields(entryView, entry);
        entryView.AutoApply = AutoApply;

        entryView.SetObject(entry);
        _entries.Add(entry);
        _entryViews.Add(entryView);

        _titlesDirty = true;
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        if (!_titlesDirty) return;
        if (_entryViews.Count == 0) return;
        if (!gameObject.activeInHierarchy) return;

        if (_updateGridCoroutine != null) StopCoroutine(_updateGridCoroutine);
        StartCoroutine(_updateGridCoroutine = UpdateGridCoroutine());
    }

    private IEnumerator UpdateGridCoroutine()
    {
        yield return null;
        yield return null;

        var newTitles = _entryViews.SelectMany(s => s.Fields).Distinct(FieldComparer.Instance).OrderBy(f => f.Index).Select(f => f.Name).ToList();
        //if (!_titles.Select(t => t.text).SequenceEqual(newTitles))
        {
            foreach (var ev in _entryViews) ev.Resize(newTitles);

            foreach (var title in _titles) Destroy(title.gameObject);
            _titles.Clear();
            for (var i = 0; i < newTitles.Count; i++)
            {
                var newTitle = newTitles[i];
                var t = Instantiate(TitleViewPrefab, TitlesContent);
                t.text = newTitle;
                _titles.Add(t);
            }
        }
        _titlesDirty = false;
        _updateGridCoroutine = null;
    }


    private void UpdateTitlesSize()
    {
        if (_entryViews.Count == 0) return;

        var sizeDelta = TitlesContent.sizeDelta;
        var x = _entryViews[0].Content.sizeDelta.x;
        //if (Math.Abs(sizeDelta.x - x) < 1f) return;

        sizeDelta.x = _entryViews[0].Content.sizeDelta.x;
        TitlesContent.sizeDelta = sizeDelta;

        var preferredWidth = Content.sizeDelta.x / _titles.Count;
        for (var i = 0; i < _titles.Count; i++)
        {
            var t = _titles[i];
            var layoutElement = t.gameObject.GetComponent<LayoutElement>() ?? t.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = preferredWidth;
        }
    }

    private class FieldComparer : IEqualityComparer<FieldAttribute>
    {
        public static FieldComparer Instance = new FieldComparer();

        public bool Equals(FieldAttribute x, FieldAttribute y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            return  x.Name == y.Name;
        }

        public int GetHashCode(FieldAttribute obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    private void RemoveEntry(int index)
    {
        CleanEntryView(_entryViews[index]);
        _entryViews.RemoveAt(index);
        _entries.RemoveAt(index);

        _titlesDirty = true;
        UpdateGrid();
    }

    private void InitEntryView(CollectionEntryView entryView)
    {
        entryView.Changed += UpdateChanged;
        entryView.Remove += RemoveEntryHandler;
        entryView.Edit += EditEntryHandler;
    }

    private void CleanEntryView(CollectionEntryView entryView)
    {
        entryView.Changed -= UpdateChanged;
        entryView.Remove -= RemoveEntryHandler;
        entryView.Edit -= EditEntryHandler;
        Destroy(entryView.gameObject);
    }

    public void AddNewClick()
    {
        var constructorInfo = Context.GetEntryMapper().Constructor;
        var entry = constructorInfo != null
            ? constructorInfo.Invoke(_entries, ConstructorParametersView.Parameters)
            : Activator.CreateInstance(Context.CollectionTargetType());

        if (entry == null) return;

        AddEntry(entry);
        IsChanged = true;
    }

    private void RemoveEntryHandler(CollectionEntryView entryView)
    {
        RemoveEntry(_entryViews.IndexOf(entryView));
        IsChanged = true;
    }

    private void EditEntryHandler(CollectionEntryView entryView)
    {
        Context.Controller.Open(_entries[_entryViews.IndexOf(entryView)]);
    }

    private void CreateEntryFields(CollectionEntryView view, object entry)
    {
        var listFieldAttribute = (CollectionFieldAttribute) Context.FieldAttribute;
        view.Context = Context;
        view.CreateEntryFields(entry, listFieldAttribute);
    }
}