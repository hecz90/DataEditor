using System;
using System.Globalization;
using DataEditor.Attributes;
using UnityEngine.UI;

public class InputFieldView : FieldView<object>
{
    public InputField Input;

    public override FieldType FieldType { get { return FieldType.Input; } }

    protected override void Awake()
    {
        base.Awake();
        Input.onValueChanged.AddListener(OnFieldChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Input.onValueChanged.RemoveListener(OnFieldChanged);
    }

    public override void SetObject(object owner)
    {
        switch (((InputFieldAttribute)Context.FieldAttribute).InputType)
        {
            case InputType.Double:
                Input.characterValidation = InputField.CharacterValidation.Decimal;
                Input.contentType = InputField.ContentType.DecimalNumber;
                break;
            case InputType.Int:
                Input.characterValidation = InputField.CharacterValidation.Integer;
                Input.contentType = InputField.ContentType.IntegerNumber;
                break;
            case InputType.String:
                Input.characterValidation = InputField.CharacterValidation.None;
                Input.contentType = InputField.ContentType.Standard;
                break;
            default: throw new NotImplementedException("Undefined InputType");
        }

        base.SetObject(owner);
    }

    public override object Value
    {
        get { return Parse(Input.text);}
        set { Input.text = value != null ? value.ToString() : string.Empty; }
    }

    public void OnFieldChanged(string arg)
    {
        UpdateChanged();
    }

    private object Parse(string str)
    {
        switch (((InputFieldAttribute)Context.FieldAttribute).InputType)
        {
            case InputType.Double:
                double doubleRes;
                return double.TryParse(str, out doubleRes) ? doubleRes : 0D;
            case InputType.Int:
                int intRes;
                return int.TryParse(str, out intRes) ? intRes : 0;
            case InputType.String:
                return str;
            default: throw  new NotImplementedException("Undefined InputType");
        }
    }

    public override void SetName(string name)
    {
        base.SetName(name);

        var placeholderText = Input.placeholder as Text;
        if (placeholderText != null) placeholderText.text = name;
    }
}