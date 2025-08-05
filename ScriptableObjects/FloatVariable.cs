using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "Variables/FloatVariable")]
public class FloatVariable : ScriptableObject
{
    public float initialValue;
    public float Value;

//     private void OnEnable()
//     {
// #if UNITY_EDITOR
//         // Unity quirk: OnEnable may be called multiple times in Edit Mode — reset only when entering Play Mode
//         if (!Application.isPlaying) return;
// #endif
//         ResetValue();
//     }

    public void ResetValue() => Value = initialValue;
}
