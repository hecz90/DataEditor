using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsView : ObjectView
{
    public Transform ButtonsContent;
    public Button ButtonPrefab;

    private List<Button> buttons = new List<Button>();
    private List<FieldView> tabViews = new List<FieldView>();

    public FieldView AddTab(string tabName, FieldView tabTemplate)
    {
        int index = buttons.Count;
        var button = Instantiate(ButtonPrefab, ButtonsContent);
        buttons.Add(button);
        button.onClick.AddListener(() => SetSelected(index));
        button.GetComponentInChildren<Text>().text = tabName;

        var fieldView = Instantiate(tabTemplate, Content);
        tabViews.Add(fieldView);

        return fieldView;

    }

    public void SetSelected(int index)
    {
        for (int i = 0; i < tabViews.Count; i++)
        {
            tabViews[i].gameObject.SetActive(index == i);
        }
    }
}