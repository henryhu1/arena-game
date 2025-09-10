using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private FloatReference countdownTimer;

    private void Update()
    {
        timerText.text = FormatTime();
    }

    private string FormatTime()
    {
        float timer = countdownTimer.Value;
        float minutes = Mathf.Floor(timer / 60);
        float seconds = Mathf.Floor(timer % 60);
        return $"{minutes}:{seconds}";
    }
}
