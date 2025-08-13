using UnityEngine;

[CreateAssetMenu(fileName = "BoundsSO", menuName = "Environment/Bounds")]
public class BoundsSO : ScriptableObject
{
    public float minX = -490;
    public float maxX = 490;
    public float minZ = -490;
    public float maxZ = 490;

    public Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);
        return position;
    }

    public bool IsInsideBounds(Vector3 position)
    {
        return position.x >= minX && position.x <= maxX &&
               position.z >= minZ && position.z <= maxZ;
    } 
}
