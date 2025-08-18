using UnityEngine;

[CreateAssetMenu(fileName = "BowData", menuName = "Items/Bow Data")]
public class BowData : WeaponData
{
    [Header("Projectile")]
    public Arrow arrowPrefab;
    public float maxForce = 100;
    public float maxSpeed = 100;

    public void FireArrow(Transform spawnPoint)
    {
        // Ensure pool exists
        ObjectPoolManager.Instance.CreatePool(arrowPrefab.gameObject, 10);

        // Spawn from pool
        GameObject arrowObj = ObjectPoolManager.Instance.Spawn(
            arrowPrefab.gameObject, 
            spawnPoint.position, 
            spawnPoint.rotation
        );

        // Launch
        if (arrowObj.TryGetComponent(out Projectile proj))
        {
            proj.Launch(MainCamera.Instance.transform.forward, maxSpeed);
        }
    }
}