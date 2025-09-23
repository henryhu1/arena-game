using UnityEngine;

[CreateAssetMenu(fileName = "BoolVariable", menuName = "Variables/BoolVariable")]
public class BoolVariable : ScriptableObject
{
    public bool initialValue;
    private bool Value;

    public void ResetValue()
    {
        SetValue(initialValue);
    }
    public void SetValue(bool newValue)
    {
        Value = newValue;
        onValueChanged.RaiseEvent(Value);
    }

    public bool GetValue() { return Value; }

    public BoolEventChannelSO onValueChanged;
}
