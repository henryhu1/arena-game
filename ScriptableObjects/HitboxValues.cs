using UnityEngine;

[CreateAssetMenu(fileName = "HitboxValues", menuName = "Attack/HitboxValues")]
public class HitboxValues : ScriptableObject
{
    public float baseDamage = 10f;

    // TODO: move start and end time for damage window to be with animation data
    public float startTime = 0.4f;
    public float endTime = 0.65f;
    public float weaponStartTime = 0.25f;
    public float weaponEndTime = 0.55f;
    public float force = 7.5f;
    public Vector3 size = new(0.7f, 1.7f, 1f);
}
