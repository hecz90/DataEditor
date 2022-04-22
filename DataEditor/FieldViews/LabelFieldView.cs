using DataEditor.Attributes;
using UnityEngine.UI;

public class LabelFieldView : FieldView<object>
{
    public Text ValueLabel;
    public string Format = "{0}";

    public override object Value
    {
        get { return _value; }
        set
        {
            _value = value;

            string valueString;
            if (_value == null)
                valueString = string.Empty;
            else if (_value.GetType().IsEnum)
                valueString = _value.GetEnumDescriptionName();
            else
                valueString = value.ToString();

            ValueLabel.text = string.Format(Format, valueString);
        }
    }
    private object _value;
    public override FieldType FieldType { get { return FieldType.Label; }
    }
    
    
}