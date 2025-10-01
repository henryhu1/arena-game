using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetData(int position, string name, float score)
    {
        positionText.text = position.ToString();
        nameText.text = name;
        scoreText.text = Mathf.Floor(score).ToString();
    }
}
