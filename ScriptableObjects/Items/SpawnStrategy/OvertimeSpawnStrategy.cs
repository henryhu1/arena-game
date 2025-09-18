using UnityEngine;

[CreateAssetMenu(menuName = "Items/Spawn Strategy/Overtime")]
public class OvertimeSpawnStrategy : ItemSpawnStrategy
{
    public float spawnChance;
    public float spawnCooldownTime;

    private float spawnCooldown;

    public override bool ShouldSpawn(float deltaTime)
    {
        spawnCooldown -= deltaTime;

        if (spawnCooldown <= 0f && Random.value > spawnChance)
        {
            spawnCooldown = spawnCooldownTime;
            return true;
        }
        return false;
    }
}
