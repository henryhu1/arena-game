using TMPro;
using UnityEngine;

public class MyCanvas : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject interactCallToAction;
    [SerializeField] private TextMeshProUGUI interactText;

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

    private void GameOverEventHandler()
    {
        gameOverMenu.SetActive(true);
    }

    private void GameRestartEventHandler()
    {
        gameOverMenu.SetActive(false);
    }
}
