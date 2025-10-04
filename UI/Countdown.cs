using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private FloatVariable countdownTimer;
    [SerializeField] private Image background;

    [SerializeField] private float colorPulseTime = 1.5f;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onGameOver;

    private Color originalColor;

    private Coroutine timeWarning;

    private void Awake()
    {
        originalColor = background.color;
    }

    private void OnEnable()
    {
        countdownTimer.onValueChanged.OnEventRaised += FormatTime;
        onGameOver.OnEventRaised += StopCountdownPulse;
    }

    private void OnDisable()
    {
        countdownTimer.onValueChanged.OnEventRaised -= FormatTime;
        onGameOver.OnEventRaised -= StopCountdownPulse;
    }

    private void FormatTime(float time)
    {
        float timer = Mathf.Max(time, 0);
        float minutes = Mathf.Floor(timer / 60);
        float seconds = Mathf.Ceil(timer % 60);
        timerText.text = $"{(minutes > 0 ? $"{minutes}:" : "")}{(0 < seconds && seconds < 10 ? "0" : "")}{seconds}";

        if (0 < time && time <= GameManager.Instance.GetWarningTimeAt())
        {
            timeWarning ??= StartCoroutine(LowTimeWarning());
        }
        else
        {
            StopCountdownPulse();
        }
    }

    private IEnumerator LowTimeWarning()
    {
        float numberOfPulses = 0;
        while (true)
        {
            Color toColor = numberOfPulses % 2 == 0 ? Color.red : originalColor;
            background.DOColor(toColor, colorPulseTime);
            yield return new WaitForSeconds(colorPulseTime);
            numberOfPulses++;
        }
    }

    private void StopCountdownPulse()
    {
        background.color = originalColor;
        if (timeWarning != null)
        {
            StopCoroutine(timeWarning);
            timeWarning = null;
        }
    }
}
