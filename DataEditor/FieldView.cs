using DataEditor.Attributes;
using DataEditor.FieldProviders;

public abstract class FieldView : View
{
    public abstract FieldType FieldType { get; }

    public string FieldName;

    public DataEditorContext Context { get; set; }
}

public abstract class FieldView<T> : FieldView
{
    public abstract T Value { get; set; }
    
    private MemberProvider<T> _field;

    public override void SetObject(object owner)
    {
        _field = CreateFieldProvider(owner);
        if (_field == null) gameObject.SetActive(false);

        Value = _field != null ? _field.GetValue() : default(T);
        IsChanged = false;
    }

    private MemberProvider<T> CreateFieldProvider(object obj)
    {
        var type = obj.GetType();
        var fieldInfo = type.GetField(FieldName);
        if (fieldInfo != null) return new FieldProvider<T>(obj, fieldInfo);

        var propertyInfo = type.GetProperty(FieldName);
        if (propertyInfo != null) return new PropertyProvider<T>(obj, propertyInfo);

        //throw new ArgumentException("obj", string.Format("[{0}] must contains field or property {1}", type.Name, FieldName));
        return null;
    }
    
    public override void Apply()
    {
        if (!IsChanged) return;

        _field.SetValue(Value);

        IsChanged = false;
    }

    protected virtual void UpdateChanged()
    {
        IsChanged = _field != null && !Equals(_field.GetValue(), Value);
    }

}