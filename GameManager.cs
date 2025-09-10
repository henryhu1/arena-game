using UnityEngine;

// TODO: stop or slow in-game time (for menus)
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private FloatReference timeElapsed;
    [SerializeField] private FloatReference countdownTimer;

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
    }

    void Update()
    {
        timeElapsed.variable.Value += Time.deltaTime;
        countdownTimer.variable.Value -= Time.deltaTime;
    }

    public float GetGameTimeElapsed() { return timeElapsed.Value; }

    public float GetCountdownTimer() { return countdownTimer.Value; }
}
