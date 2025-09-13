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

    [SerializeField] private float warningTimeAt = 10f;

    [SerializeField] private float colorPulseTime = 1.5f;

    private Color originalColor;

    private Coroutine timeWarning;

    private void Awake()
    {
        originalColor = background.color;
    }

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
        timerText.text = $"{(minutes > 0 ? $"{minutes}:" : "")}{(0 < seconds && seconds < 10 ? "0" : "")}{seconds}";

        if (0 < time && time <= warningTimeAt)
        {
            timeWarning ??= StartCoroutine(LowTimeWarning());
        }
        else
        {
            background.color = originalColor;
            if (timeWarning != null)
            {
                StopCoroutine(timeWarning);
                timeWarning = null;
            }
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
}
