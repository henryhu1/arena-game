using UnityEngine;

[CreateAssetMenu(fileName = "HitboxValues", menuName = "Player/Attack/HitboxValues")]
public class HitboxValues : ScriptableObject
{
    public float baseDamage = 10f;
    public float force = 7.5f;
    public Vector3 size = new(0.7f, 1.7f, 1f);
}
