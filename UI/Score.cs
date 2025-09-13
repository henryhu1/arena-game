using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private FloatVariable playerScore;

    private void Start()
    {
        scoreText.text = Mathf.Floor(playerScore.GetValue()).ToString();
    }
    private void OnEnable()
    {
        playerScore.onValueChanged.OnEventRaised += UpdateScoreText;
    }

    private void OnDisable()
    {
        playerScore.onValueChanged.OnEventRaised -= UpdateScoreText;
    }

    private void UpdateScoreText(float score)
    {
        scoreText.text = Mathf.Floor(score).ToString();
    }
}
