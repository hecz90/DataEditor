using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataEditor.FieldProviders;
using DataEditor.Mapping;
using UnityEngine;
using UnityEngine.UI;

public class DataEditorController : MonoBehaviour
{
    public RectTransform Content;
    public ObjectView RootPrefab;

    public List<FieldView> Templates;
    public TabsView TabsView;
    public Button ApplyButton;

    public RectTransform ButtonsContant;
    public Button ActionButtonPrefab;

    private List<Button> _actionButtons = new List<Button>();


    private readonly Stack<KeyValuePair<object, ObjectView>> _stackOpened = new Stack<KeyValuePair<object, ObjectView>>();
    private object _currentObject;
    private ObjectView _root;

    public event Action Close;

    public void Open(object next)
    {
        if (_currentObject != null) _stackOpened.Push(new KeyValuePair<object, ObjectView>(_currentObject, _root));

        var root = Instantiate(RootPrefab, Content);
        CreateFields(next, root);

        OpenInternal(next, root);
    }


    public void Back()
    {
        if (_stackOpened.Count == 0)
        {
            Close?.Invoke();
            return;
        }

        _root.Changed -= OnRootOnChanged;
        _root.Clear();
        Destroy(_root.gameObject);
        _root = null;

        var kv = _stackOpened.Pop();
        OpenInternal(kv.Key, kv.Value);

    }


    private void OpenInternal(object next, ObjectView root)
    {
        _currentObject = next;

        if (_root)
        {
            _root.gameObject.SetActive(false);
            _root.Changed -= OnRootOnChanged;
        }

        _root = root;
        _root.gameObject.SetActive(true);

        CreateActionButtons(_currentObject);

        _root.SetObject(_currentObject);

        _root.Changed += OnRootOnChanged;
    }

    void OnDisable()
    {
        Clear();
    }

    void Awake()
    {
        ApplyButton.onClick.AddListener(() => _root.Apply());
    }

    public void Clear()
    {
        foreach (var kv in _stackOpened) Destroy(kv.Value);
        _stackOpened.Clear();

        _currentObject = null;
        if (_root)
        {
            _root.Changed -= OnRootOnChanged;
            _root.Clear();
            Destroy(_root.gameObject);
        }

        ClearActionButtons();
    }

    private void ClearActionButtons()
    {
        foreach (var button in _actionButtons)
        {
            Destroy(button.gameObject);
        }
        _actionButtons.Clear();
    }

    private void CreateFields(object owner, IObjectView parentView)
    {
        var mapper = DataMapper.GetMapper(owner.GetType());

        TabsView tabs = null;
        bool tabsCreated = false;
        bool needApplyButton = false;
        foreach (var data in mapper.Members)
        {
            var memberInfo = data.Info;
            var fieldAttribute = data.FieldAttribute;
            if (fieldAttribute == null) continue;

            var fieldType = fieldAttribute.FieldType;
            var fieldTemplate = Templates.Find(t => t.FieldType == fieldType);
            var memberProvider = MemberProvider<object>.Create(owner, memberInfo);

            FieldView fieldView;

            var tabAttribute = data.TabAttribute;
            if (tabAttribute != null)
            {
                if (tabs == null)
                {
                    tabsCreated = true;
                    tabs = Instantiate(TabsView, parentView.Content);
                    tabs.transform.SetSiblingIndex(0);
                    parentView.AddChild(tabs);
                }

                fieldView = tabs.AddTab(fieldAttribute.Name, fieldTemplate);
                tabs.AddChild(fieldView);
            }
            else
            {
                fieldView = Instantiate(fieldTemplate, parentView.Content);
                parentView.AddChild(fieldView);
            }

            fieldView.FieldName = memberInfo.Name;
            fieldView.name = fieldView.name = fieldTemplate.name + " [" + memberInfo.Name + "]";
            fieldView.SetName(fieldAttribute.Name);
            fieldView.Context = new DataEditorContext { Controller = this, FieldAttribute = fieldAttribute };
            fieldView.AutoApply = fieldAttribute.AutoApply;
            needApplyButton |= !fieldView.AutoApply;

            var objectView = fieldView as IObjectView;
            if (objectView != null)
            {
                CreateFields(memberProvider.GetValue(), objectView);
            }
        }

        parentView.AutoApply |= !needApplyButton;

        if (tabsCreated)
        {
            StartCoroutine(TabStartCoroutine(tabs));
        }
    }

    private void CreateActionButtons(object owner)
    {
        ClearActionButtons();

        var mapper = DataMapper.GetMapper(owner.GetType());

        ApplyButton.gameObject.SetActive(mapper.Members.Any(m => !m.FieldAttribute.AutoApply));

        foreach (var kv in mapper.Actions)
        {
            var methodInfo = kv.Key;
            var actionAttribute = kv.Value;

            var button = Instantiate(ActionButtonPrefab, ButtonsContant);
            _actionButtons.Add(button);

            button.GetComponentInChildren<Text>().text = actionAttribute.Name;
            button.onClick.AddListener(() => methodInfo.Invoke(null, null));
        }
    }

    private static IEnumerator TabStartCoroutine(TabsView tabs)
    {
        yield return null;
        yield return null;
        tabs.SetSelected(0);
    }

    private void OnRootOnChanged()
    {
        ApplyButton.interactable = _root.IsChanged;
    }

}