using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IView
{
    event Action Changed;
    bool IsChanged { get; }
    void SetObject(object owner);
    void SetName(string name);
    void Apply();

}


public abstract class View : UIBehaviour, IView
{
    public Text Label;
    public bool AutoApply;

    public event Action Changed;

    public bool IsChanged
    {
        get { return _changed; }
        protected set
        {
            if (_changed == value) return;

            _changed = value;
           if (Changed != null) Changed();

           if (AutoApply && value) Apply();
        }
    }
    private bool _changed;

    public abstract void SetObject(object owner);
    public virtual void SetName(string name)
    {
        if (Label) Label.text = name;
    }

    public abstract void Apply();
}