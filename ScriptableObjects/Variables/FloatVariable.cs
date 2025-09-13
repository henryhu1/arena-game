using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "Variables/FloatVariable")]
public class FloatVariable : ScriptableObject
{
    public float initialValue;
    private float Value;
    public void ResetValue() => Value = initialValue;
    public void AddToValue(float delta)
    {
        Value += delta;
        onValueChanged.RaiseEvent(Value);
    }
    public void SubtractFromValue(float delta)
    {
        Value -= delta;
        onValueChanged.RaiseEvent(Value);
    }

    public float GetValue() { return Value; }

    public FloatEventChannelSO onValueChanged;
}
