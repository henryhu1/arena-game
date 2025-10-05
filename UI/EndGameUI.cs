using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private FloatVariable PlayerScore;
    [SerializeField] private FloatVariable InGameTime;
    [SerializeField] private IntVariable EnemiesDefeated;
    [SerializeField] private IntVariable CurrentRound;

    [Header("UI")]
    [SerializeField] private GameObject nameInputObject;
    [SerializeField] private GameObject submitButtonObject;
    [SerializeField] private GameObject errorIndicatorObject;
    [SerializeField] private Leaderboard leaderboard;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button submitButton;

    [Header("Input")]
    [SerializeField] private TMP_InputField nameInput;

    private string playerName;
    private LeaderboardList scoresAroundPlayer;
    private LeaderboardList topScores;

    private void Awake()
    {
        gameObject.SetActive(false);
        Initialize();
    }

    private void Initialize()
    {
        HideScoreSubmission();
        HideLeaderboard();
    }

    private void OnEnable()
    {
        if (CurrentRound.GetValue() >= 2)
        {
            AllowScoreSubmission();
        }

        submitButton.onClick.AddListener(OnSubmitScore);
        nameInput.onValueChanged.AddListener(UpdatePlayerName);
    }

    private void OnDisable()
    {
        HideScoreSubmission();
        HideLeaderboard();
        submitButton.onClick.RemoveListener(OnSubmitScore);
        nameInput.onValueChanged.RemoveListener(UpdatePlayerName);
    }

    private void UpdatePlayerName(string value)
    {
        playerName = value;
    }

    private void OnSubmitScore()
    {
        if (playerName == null || playerName == "") return;

        DisableScoreSubmission();
        SubmitScoreData data = new(
            playerName,
            PlayerScore.GetValue(),
            CurrentRound.GetValue(),
            EnemiesDefeated.GetValue(),
            InGameTime.GetValue()
        );
        StartCoroutine(PostToApi(data));
    }

    private IEnumerator PostToApi(SubmitScoreData data)
    {
        yield return StartCoroutine(
            ApiClient.Instance.PostPlayerScore(
                data,
                docId => StartCoroutine(GetFromApi(docId)),
                (errorString) =>
                {
                    submitButton.interactable = true;
                    errorIndicatorObject.SetActive(true);
                    errorText.text = errorString;
                }
            )
        );
    }

    private IEnumerator GetFromApi(string docId)
    {
        leaderboard.SetPlayerId(docId);

        yield return StartCoroutine(
            ApiClient.Instance.GetPlayerOnLeaderboard(
                docId,
                closeScores =>
                {
                    scoresAroundPlayer = closeScores;
                    ShowLeaderboard();
                    leaderboard.SetProximatePlayerScores(scoresAroundPlayer);
                }
            )
        );

        yield return StartCoroutine(
            ApiClient.Instance.GetLeaderboardTop(
                topOfLeaderboard =>
                {
                    topScores = topOfLeaderboard;
                    ShowLeaderboard();
                    leaderboard.SetTopScores(topScores);
                }
            )
        );
    }

    private void AllowScoreSubmission()
    {
        nameInputObject.SetActive(true);
        submitButtonObject.SetActive(true);
        submitButton.interactable = true;
    }

    private void DisableScoreSubmission()
    {
        submitButton.interactable = false;
        errorIndicatorObject.SetActive(false);
    }

    private void HideScoreSubmission()
    {
        nameInputObject.SetActive(false);
        submitButtonObject.SetActive(false);
        errorIndicatorObject.SetActive(false);
        errorText.text = "!";
    }

    private void ShowLeaderboard()
    {
        leaderboard.gameObject.SetActive(true);
    }

    private void HideLeaderboard()
    {
        leaderboard.gameObject.SetActive(false);
    }
}
