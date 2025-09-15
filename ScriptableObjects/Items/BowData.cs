using UnityEngine;

[CreateAssetMenu(fileName = "BowData", menuName = "Items/Bow Data")]
public class BowData : WeaponData
{
    [Header("Projectile")]
    public GameObject arrowPrefab;

    [Header("Projectile Stats")]
    public float maxSpeed = 100;

    public void FireArrow(Transform spawnPoint)
    {
        // Ensure pool exists
        ObjectPoolManager.Instance.CreatePool(arrowPrefab, 10);

        // Spawn from pool
        GameObject arrowObj = ObjectPoolManager.Instance.Spawn(
            arrowPrefab.gameObject,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Launch
        if (arrowObj.TryGetComponent(out IProjectilible proj))
        {
            proj.Launch(this, MainCamera.Instance.transform.forward, maxSpeed);
        }
    }

    public void ReturnArrowToPool(GameObject arrowObj)
    {
        ObjectPoolManager.Instance.Despawn(arrowObj, arrowPrefab);
    }
}