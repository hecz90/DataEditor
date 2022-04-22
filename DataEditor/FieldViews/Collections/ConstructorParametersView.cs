using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;



public class ConstructorParametersView : UIBehaviour
{
    class ConstructorParameter
    {
        public object Parameter;
    }

    public RectTransform Content;
    public List<FieldView> Templates;

    private readonly List<FieldView> _parameterViews = new List<FieldView>();
    private readonly List<ConstructorParameter> _parameters = new List<ConstructorParameter>();

    public List<object> Parameters {  get { return _parameters.Select(p => p.Parameter).ToList(); } }

    public void Clear()
    {
        foreach (var parameterView in _parameterViews)
        {
            Destroy(parameterView.gameObject);
        }
        _parameterViews.Clear();
        _parameters.Clear();
    }
    
    public void CreateParameterViews(DataEditorContext collectionContext)
    {
        Clear();

        var mapper = collectionContext.GetEntryMapper();
        var constructor = mapper.Constructor;

        if (constructor == null) return;

        foreach (var kv in constructor.Parameters)
        {
            var parameterInfo = kv.Key;
            var fieldAttribute = kv.Value;
            var fieldType = fieldAttribute.FieldType;

            var fieldTemplate = Templates.Find(t => t.FieldType == fieldType);
            if (fieldTemplate == null) throw new ArgumentException(string.Format("has not template for parameter {0}.{1}({2}[{3}])", mapper.Type.Name, constructor.Name, parameterInfo.Name, fieldType));

            var fieldView = Instantiate(fieldTemplate, Content);
            fieldView.FieldName = "Parameter";
            fieldView.name = fieldView.name = fieldTemplate.name + " [" + parameterInfo.Name + "]";
            fieldView.SetName(fieldAttribute.Name);
            fieldView.Context = new DataEditorContext { Controller = collectionContext.Controller, FieldAttribute = fieldAttribute };
            fieldView.AutoApply = true;
            _parameterViews.Add(fieldView);
            
            var parameter = new ConstructorParameter {Parameter = Activator.CreateInstance(parameterInfo.ParameterType)};
            _parameters.Add(parameter);

            fieldView.SetObject(parameter);
        }
    }
}