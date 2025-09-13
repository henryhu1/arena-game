using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private FloatVariable countdownTimer;

    private void Start()
    {
        FormatTime(countdownTimer.GetValue());
    }

    private void OnEnable()
    {
        countdownTimer.onValueChanged.OnEventRaised += FormatTime;
    }

    private void OnDisable()
    {
        countdownTimer.onValueChanged.OnEventRaised -= FormatTime;
    }

    private void FormatTime(float time)
    {
        float timer = Mathf.Max(time, 0);
        float minutes = Mathf.Floor(timer / 60);
        float seconds = Mathf.Floor(timer % 60);
        timerText.text = $"{(minutes > 0 ? $"{minutes}:" : "")}{(seconds < 10 ? "0" : "")}{seconds}";
    }
}
