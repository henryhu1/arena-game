using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class MyCanvas : MonoBehaviour
{
    public static MyCanvas Instance;

    [Header("UI")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePausedMenu;
    [SerializeField] private GameObject interactCallToAction;
    [SerializeField] private TextMeshProUGUI interactText;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onGameOver;
    [SerializeField] private VoidEventChannelSO onGameRestart;
    [SerializeField] private BoolEventChannelSO onGamePauseToggle;

    [Header("Audio")]
    [SerializeField] private AudioEffectSO arrowEffect;
    [SerializeField] private AudioEffectSO timerDecreaseEffect;
    [SerializeField] private AudioEffectSO unpauseEffect;
    [SerializeField] private AudioEffectSO pauseEffect;

    private StringTable controlsTable;

    private AudioSource audioSource;

    private async void Awake()
    {
        if (Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        controlsTable = await LocalizationSettings.StringDatabase.GetTableAsync("Controls").Task;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
        if (GameManager.Instance.IsGameOver() || interactable == null || !interactable.IsInteractable())
        {
            interactCallToAction.SetActive(false);
        }
        else
        {
            interactCallToAction.SetActive(true);
            Vector3 screenPos = interactable.GetScreenPos();
            interactCallToAction.transform.position = screenPos;
            string localizedInteraction = controlsTable.GetEntry(interactable.GetInteractionTextKey()).GetLocalizedString();
            interactText.text = localizedInteraction;
        }
    }

    private void GamePausedToggleHandler(bool isPaused)
    {
        gamePausedMenu.SetActive(isPaused);
    }

    private void GameOverEventHandler()
    {
        gameOverScreen.SetActive(true);
    }

    private void GameRestartEventHandler()
    {
        gameOverScreen.SetActive(false);
    }

    public void PlayAudio(AudioEffectSO audioEffect)
    {
        audioSource.PlayOneShot(audioEffect.GetRandomClip());
    }

    public void PlayArrowAudio()
    {
        audioSource.PlayOneShot(arrowEffect.GetRandomClip());
    }

    public void PlayTimerDecreaseEffect()
    {
        audioSource.PlayOneShot(timerDecreaseEffect.GetRandomClip());
    }

    public void PlayPauseAudio()
    {
        audioSource.PlayOneShot(pauseEffect.GetRandomClip());
    }

    public void PlayUnpauseAudio()
    {
        audioSource.PlayOneShot(unpauseEffect.GetRandomClip());
    }
}
