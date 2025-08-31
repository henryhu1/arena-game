using UnityEngine;

public enum EnemyMaxAliveIncrease
{
    ADDITIVE,
    MULTIPLICATIVE,
}

[CreateAssetMenu(fileName = "EnemySpawnData", menuName = "Enemy/SpawnData")]
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

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        ResetCurrentAlive();
    }

    public int GetMaxAliveForRound(int round)
    {
        if (maxIncreaseType == EnemyMaxAliveIncrease.ADDITIVE)
        {
            return Mathf.FloorToInt(baseMaxAlive + increaseRate * round);
        }
        else
        {
            return Mathf.FloorToInt(baseMaxAlive * Mathf.Pow(increaseRate, round));
        }
    }

    public void ResetCurrentAlive() => currentAlive = 0;
}
