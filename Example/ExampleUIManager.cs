using System.Globalization;
using UnityEngine;
using Dictionary = Example.Data.Dictionary;

public class ExampleUIManager : MonoBehaviour
{
    public DataEditorController DataEditorController;

    void Awake()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        DataEditorController.Open(new Dictionary());
    }
}