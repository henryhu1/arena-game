using TMPro;
using UnityEngine;

public class MyCanvas : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject gamePausedMenu;
    [SerializeField] private GameObject interactCallToAction;
    [SerializeField] private TextMeshProUGUI interactText;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onGameOver;
    [SerializeField] private VoidEventChannelSO onGameRestart;
    [SerializeField] private BoolEventChannelSO onGamePauseToggle;

    private void OnEnable()
    {
        onGamePauseToggle.OnEventRaised += GamePausedToggleHandler;
        onGameOver.OnEventRaised += GameOverEventHandler;
        onGameRestart.OnEventRaised += GameRestartEventHandler;
    }

    private void OnDisable()
    {
        onGamePauseToggle.OnEventRaised -= GamePausedToggleHandler;
        onGameOver.OnEventRaised -= GameOverEventHandler;
        onGameRestart.OnEventRaised -= GameRestartEventHandler;
    }

    void Update()
    {
        IInteractable interactable = PlayerManager.Instance.interactHandler.GetClosestInteractable();
        if (interactable == null || !interactable.IsInteractable())
        {
            interactCallToAction.SetActive(false);
        }
        else
        {
            interactCallToAction.SetActive(true);
            Vector3 screenPos = interactable.GetScreenPos();
            interactCallToAction.transform.position = screenPos;
            interactText.text = interactable.GetInteractionText();
        }
    }

    private void GamePausedToggleHandler(bool isPaused)
    {
        gamePausedMenu.SetActive(isPaused);
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
