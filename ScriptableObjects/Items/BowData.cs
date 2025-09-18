using UnityEngine;

[CreateAssetMenu(fileName = "BowData", menuName = "Items/Bow Data")]
public class BowData : WeaponData
{
    [Header("Projectile Stats")]
    public float maxSpeed = 100;

    public void FireArrow(Transform spawnPoint)
    {
        // Spawn from pool
        GameObject arrowObj = ObjectPoolManager.Instance.SpawnArrow(
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Launch
        if (arrowObj.TryGetComponent(out IProjectilible proj))
        {
            proj.Launch(this, MainCamera.Instance.transform.forward, maxSpeed);
        }
    }
}