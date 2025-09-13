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
    [SerializeField] private VoidEventChannelSO onGameOver;

    private bool isCountingDown = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        timeElapsed.ResetValue();
        countdownTimer.ResetValue();
        playerScore.ResetValue();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isCountingDown = false;
    }

    void OnEnable()
    {
        onRoundStart.OnEventRaised += RoundStarted;
        onRoundEnd.OnEventRaised += RoundEnded;
        onEnemyDefeated.OnEnemyEvent += EnemyDefeated;
    }

    void OnDisable()
    {
        onRoundStart.OnEventRaised -= RoundStarted;
        onRoundEnd.OnEventRaised -= RoundEnded;
        onEnemyDefeated.OnEnemyEvent -= EnemyDefeated;
    }

    void Update()
    {
        timeElapsed.AddToValue(Time.deltaTime);
        if (isCountingDown)
        {
            countdownTimer.SubtractFromValue(Time.deltaTime);
        }
        if (countdownTimer.GetValue() <= 0)
        {
            GameOver();
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
        playerScore.AddToValue(enemy.GetEnemyStats().PointValue());
        countdownTimer.AddToValue(enemy.GetEnemyStats().TimeRegained());
    }

    private void GameOver()
    {
        onGameOver.RaiseEvent();
    }

    public float GetGameTimeElapsed() { return timeElapsed.GetValue(); }

    public float GetCountdownTimer() { return countdownTimer.GetValue(); }
}
