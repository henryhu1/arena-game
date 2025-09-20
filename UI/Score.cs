using DG.Tweening;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private FloatVariable playerScore;
    [SerializeField] private RectTransform rect;
    [SerializeField] private RectTransform canvasRect;

    [SerializeField] private float transitionTime = 1.5f;

    private float originalHeight;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onGameOver;
    [SerializeField] private VoidEventChannelSO onGameRestart;

    private void Awake()
    {
        originalHeight = rect.anchoredPosition.y;
    }

    private void Start()
    {
        scoreText.text = Mathf.Floor(playerScore.GetValue()).ToString();
    }
    private void OnEnable()
    {
        playerScore.onValueChanged.OnEventRaised += UpdateScoreText;
        onGameOver.OnEventRaised += MoveToCentre;
        onGameRestart.OnEventRaised += MoveToOriginalPosition;
    }

    private void OnDisable()
    {
        playerScore.onValueChanged.OnEventRaised -= UpdateScoreText;
        onGameOver.OnEventRaised -= MoveToCentre;
        onGameRestart.OnEventRaised -= MoveToOriginalPosition;
    }

    private void UpdateScoreText(float score)
    {
        scoreText.text = Mathf.Floor(score).ToString();
    }

    private void MoveToCentre()
    {
        rect.DOMoveY(Screen.height / 2, transitionTime).SetEase(Ease.InOutQuart);
    }

    private void MoveToOriginalPosition()
    {
        rect.DOMoveY(Screen.height + originalHeight, transitionTime).SetEase(Ease.InOutQuart);
    }
}
