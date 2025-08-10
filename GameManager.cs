using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FloatReference timeElapsed;

    void Awake()
    {
        timeElapsed.variable.ResetValue();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        timeElapsed.variable.Value += Time.deltaTime;
    }

    public float GetGameTimeElapsed() { return timeElapsed.Value; }
}
