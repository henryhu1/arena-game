using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "Variables/FloatVariable")]
public class FloatVariable : ScriptableObject
{
    public float initialValue;
    public float Value;
    public void ResetValue() => Value = initialValue;
}
