using UnityEngine;

public class MyCanvas : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverMenu;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onGameOver;
    [SerializeField] private VoidEventChannelSO onGameRestart;

    private void OnEnable()
    {
        onGameOver.OnEventRaised += GameOverEventHandler;
        onGameRestart.OnEventRaised += GameRestartEventHandler;
    }

    private void OnDisable()
    {
        onGameOver.OnEventRaised -= GameOverEventHandler;
        onGameRestart.OnEventRaised -= GameRestartEventHandler;
    }

    private void GameOverEventHandler()
    {
        gameOverMenu.SetActive(true);
    }

    private void GameRestartEventHandler()
    {
        gameOverMenu.SetActive(false);
    }
}
