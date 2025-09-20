using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private GameObject nameInputObject;
    [SerializeField] private GameObject submitButtonObject;
    [SerializeField] private Button restartButton;
    [SerializeField] private float transitionTime = 1.5f;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        gameObject.SetActive(false);
        HideScoreSubmission();
    }

    private void OnEnable()
    {
        // rect.DOMoveY(Screen.height / 2, transitionTime);

        if (GameRoundManager.Instance.CurrentRound > 1)
        {
            AllowScoreSubmission();
        }
    }

    private void OnDisable()
    {
        // rect.DOMoveY(Screen.height, transitionTime);
        HideScoreSubmission();
    }

    private void AllowScoreSubmission()
    {
        nameInputObject.SetActive(true);
        submitButtonObject.SetActive(true);
    }

    private void HideScoreSubmission()
    {
        nameInputObject.SetActive(false);
        submitButtonObject.SetActive(false);
    }
}
