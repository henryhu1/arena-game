using UnityEngine;
using System.Collections;

public class GameRoundManager : MonoBehaviour
{
    public static GameRoundManager Instance { get; private set; }

    public float timeBetweenRounds = 5f;
    [SerializeField] private IntVariable currentRound;

    private Coroutine roundBuffer;

    [Header("Events")]
    [SerializeField] private IntEventChannelSO roundEndedEventChannel;
    [SerializeField] private VoidEventChannelSO allWaveEnemiesDefeatedEventChannel;
    [SerializeField] private VoidEventChannelSO onGameOver;
    [SerializeField] private VoidEventChannelSO onGameRestart;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
        currentRound.ResetValue();
    }

    private void Start()
    {
        IncrementRound();
    }

    private void OnEnable()
    {
        allWaveEnemiesDefeatedEventChannel.OnEventRaised += IncrementRound;
        onGameOver.OnEventRaised += GameOverEventHandler;
        onGameRestart.OnEventRaised += RestartGame;
    }

    private void OnDisable()
    {
        allWaveEnemiesDefeatedEventChannel.OnEventRaised -= IncrementRound;
        onGameOver.OnEventRaised -= GameOverEventHandler;
        onGameRestart.OnEventRaised -= RestartGame;
    }

    public void IncrementRound()
    {
        Debug.Log($"Wave {currentRound.GetValue()} completed. Next wave in {timeBetweenRounds} seconds...");
        roundEndedEventChannel.RaiseEvent(currentRound.GetValue());
        roundBuffer = StartCoroutine(BetweenRoundsBuffer());
    }

    private IEnumerator BetweenRoundsBuffer()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        currentRound.AddToValue(1);
        Debug.Log($"Wave {currentRound.GetValue()} starting.");
    }

    private void GameOverEventHandler()
    {
        if (roundBuffer != null)
        {
            StopCoroutine(roundBuffer);
        }
    }

    private void RestartGame()
    {
        currentRound.ResetValue();
        IncrementRound();
    }
}
