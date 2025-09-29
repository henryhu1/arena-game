using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "Variables/IntVariable")]
public class IntVariable : ScriptableObject
{
    public int initialValue;
    private int Value;
    public void ResetValue()
    {
        Value = initialValue;
        onValueChanged.RaiseEvent(Value);
    }
    public void AddToValue(int delta)
    {
        Value += delta;
        onValueChanged.RaiseEvent(Value);
    }
    public void SubtractFromValue(int delta)
    {
        Value -= delta;
        onValueChanged.RaiseEvent(Value);
    }

    public int GetValue() { return Value; }

    public IntEventChannelSO onValueChanged;
}
