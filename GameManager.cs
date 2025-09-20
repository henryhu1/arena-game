using UnityEngine;

// TODO: stop or slow in-game time (for menus)
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private FloatVariable timeElapsed;
    [SerializeField] private FloatVariable countdownTimer;
    [SerializeField] private FloatVariable playerScore;

    [Header("Events")]
    [SerializeField] private IntEventChannelSO onRoundStart;
    [SerializeField] private IntEventChannelSO onRoundEnd;
    [SerializeField] private EnemyEventChannelSO onEnemyDefeated;
    [SerializeField] private VoidEventChannelSO onPlayerDeath;
    [SerializeField] private VoidEventChannelSO onTimeRunOut;
    [SerializeField] private VoidEventChannelSO onGameOver;
    [SerializeField] private VoidEventChannelSO onGameRestart;

    private bool isCountingDown = false;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    public void RestartGame()
    {
        onGameRestart.RaiseEvent();
        StartGame();
    }

    private void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isCountingDown = false;
        isGameOver = false;

        timeElapsed.ResetValue();
        countdownTimer.ResetValue();
        playerScore.ResetValue();
    }

    void OnEnable()
    {
        onRoundStart.OnEventRaised += RoundStarted;
        onRoundEnd.OnEventRaised += RoundEnded;
        onEnemyDefeated.OnEnemyEvent += EnemyDefeated;
        onPlayerDeath.OnEventRaised += PlayerDied;
    }

    void OnDisable()
    {
        onRoundStart.OnEventRaised -= RoundStarted;
        onRoundEnd.OnEventRaised -= RoundEnded;
        onEnemyDefeated.OnEnemyEvent -= EnemyDefeated;
        onPlayerDeath.OnEventRaised -= PlayerDied;
    }

    void Update()
    {
        timeElapsed.AddToValue(Time.deltaTime);
        if (isCountingDown)
        {
            countdownTimer.SubtractFromValue(Time.deltaTime);
            if (countdownTimer.GetValue() <= 0)
            {
                TimeRanOut();
            }
        }

    }

    private void RoundStarted(int _)
    {
        isCountingDown = true;
    }

    private void RoundEnded(int _)
    {
        isCountingDown = false;
    }

    private void EnemyDefeated(EnemyControllerBase enemy)
    {
        if (!isGameOver)
        {
            playerScore.AddToValue(enemy.GetPointValue());
            countdownTimer.AddToValue(enemy.GetTimeRegained());
        }
    }

    private void PlayerDied()
    {
        GameOver();
    }

    private void TimeRanOut()
    {
        onTimeRunOut.RaiseEvent();
        GameOver();
    }

    private void GameOver()
    {
        onGameOver.RaiseEvent();
        isGameOver = true;
        isCountingDown = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public float GetGameTimeElapsed() { return timeElapsed.GetValue(); }

    public float GetCountdownTimer() { return countdownTimer.GetValue(); }
    public bool IsGameOver() { return isGameOver; }
}
