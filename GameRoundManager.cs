using UnityEngine;
using System.Collections;

public class GameRoundManager : MonoBehaviour
{
    public static GameRoundManager Instance { get; private set; }

    public int CurrentRound { get; private set; } = 1;
    public float timeBetweenRounds = 5f;

    public EnemySpawner spawner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        StartCoroutine(RoundLoop());
    }

    private IEnumerator RoundLoop()
    {
        while (true)
        {
            Debug.Log($"Wave {CurrentRound} starting.");
            spawner.SpawnWave(CurrentRound);

            // Wait until all enemies are dead
            yield return new WaitUntil(() => spawner.TotalWaveEnemiesAlive == 0);

            Debug.Log($"Wave {CurrentRound} completed. Next wave in {timeBetweenRounds} seconds...");
            yield return new WaitForSeconds(timeBetweenRounds);

            CurrentRound++;
        }
    }
}
