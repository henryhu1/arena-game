using UnityEngine;

[CreateAssetMenu(fileName = "HitboxValues", menuName = "Attack/HitboxValues")]
public class HitboxValues : ScriptableObject
{
    public float baseDamage = 10f;
    public float startTime = 0.6f;
    public float endTime = 0.8f;
    public float force = 7.5f;
}
