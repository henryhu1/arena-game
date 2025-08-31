using UnityEngine;

// TODO: particle effects for hits, enemy spawns, death, movement
// TODO: stop or slow in-game time (for menus)
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
