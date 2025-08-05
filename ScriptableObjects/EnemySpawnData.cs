using UnityEngine;

public enum EnemyMaxAliveIncrease
{
    ADDITIVE,
    MULTIPLICATIVE,
}

[CreateAssetMenu(fileName = "EnemySpawnData", menuName = "Enemy/EnemySpawnData")]
public class EnemySpawnData : ScriptableObject
{
    public EnemySpawnStrategy spawnStrategy;
    public GameObject enemyPrefab;
    public float minDistanceFromPlayer = 10f;
    public float maxDistanceFromPlayer = 20f;
    public int baseMaxAlive = 10;
    public EnemyMaxAliveIncrease maxIncreaseType = EnemyMaxAliveIncrease.ADDITIVE;
    public float increaseRate = 2;

    public int currentAlive = 0;

    public int GetMaxAliveForRound(int round)
    {
        if (maxIncreaseType == EnemyMaxAliveIncrease.ADDITIVE)
        {
            return Mathf.RoundToInt(baseMaxAlive + increaseRate * round);
        }
        else
        {
            return Mathf.RoundToInt(baseMaxAlive * Mathf.Pow(increaseRate, round));
        }
    }
}
