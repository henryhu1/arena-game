using UnityEngine;

// TODO: stop or slow in-game time (for menus)
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private FloatReference timeElapsed;
    [SerializeField] private FloatReference countdownTimer;
    [SerializeField] private FloatReference pointsEarned;

    [Header("Events")]
    [SerializeField] private IntEventChannelSO onRoundStart;
    [SerializeField] private IntEventChannelSO onRoundEnd;
    [SerializeField] private EnemyEventChannelSO onEnemyDefeated;

    private bool isCountingDown = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        timeElapsed.variable.ResetValue();
        countdownTimer.variable.ResetValue();
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
        timeElapsed.variable.Value += Time.deltaTime;
        if (isCountingDown)
        {
            countdownTimer.variable.Value -= Time.deltaTime;
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
        pointsEarned.variable.Value += enemy.GetEnemyStats().PointValue();
        countdownTimer.variable.Value += enemy.GetEnemyStats().TimeRegained();
    }

    public float GetGameTimeElapsed() { return timeElapsed.Value; }

    public float GetCountdownTimer() { return countdownTimer.Value; }
}
