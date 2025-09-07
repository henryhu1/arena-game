using UnityEngine;
using System.Collections;

public class GameRoundManager : MonoBehaviour
{
    public static GameRoundManager Instance { get; private set; }

    public int CurrentRound { get; private set; } = 0;
    public float timeBetweenRounds = 5f;

    [Header("Events")]
    [SerializeField] private IntEventChannelSO roundStartedEventChannel;
    [SerializeField] private IntEventChannelSO roundEndedEventChannel;
    [SerializeField] private VoidEventChannelSO allWaveEnemiesDefeatedEventChannel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    private void Start()
    {
        allWaveEnemiesDefeatedEventChannel.OnEventRaised += IncrementRound;
        IncrementRound();
    }

    private void OnDestroy()
    {
        allWaveEnemiesDefeatedEventChannel.OnEventRaised -= IncrementRound;
    }

    public void IncrementRound()
    {
        Debug.Log($"Wave {CurrentRound} completed. Next wave in {timeBetweenRounds} seconds...");
        roundEndedEventChannel.RaiseEvent(CurrentRound);
        StartCoroutine(BetweenRoundsBuffer());
    }

    private IEnumerator BetweenRoundsBuffer()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        Debug.Log($"Wave {CurrentRound} starting.");
        CurrentRound++;
        roundStartedEventChannel.RaiseEvent(CurrentRound);
    }
}
