using DG.Tweening;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform entireRect;
    [SerializeField] private RectTransform bar;
    [SerializeField] private RectTransform changeBar;

    [Header("Animation")]
    [SerializeField] private float shakeTime = 1f;
    [SerializeField] private float shakeMaxDistance = 2f;
    [SerializeField] private int vibrato = 2;
    [SerializeField] private float randomness = 45;
    [SerializeField] private float decreaseBarTime = 1f;

    private float originalWidth;
    private Tween healthBarAnimation;
    private Sequence healthBarDecreaseSequence;

    [Header("Data")]
    [SerializeField] private FloatVariable playerHealth;

    private void Awake()
    {
        originalWidth = bar.sizeDelta.x;
    }

    private void OnEnable()
    {
        playerHealth.onValueChanged.OnEventRaised += UpdateBarWidth;
    }

    private void OnDisable()
    {
        playerHealth.onValueChanged.OnEventRaised -= UpdateBarWidth;
    }

    private void UpdateBarWidth(float healthPoints)
    {
        float currentWidth = bar.sizeDelta.x;
        float newWidth = Mathf.Max(originalWidth * healthPoints / playerHealth.initialValue, 0);

        if (newWidth < currentWidth)
        {
            float diff = currentWidth - newWidth;
            float strength = Mathf.Max(diff / playerHealth.initialValue * shakeMaxDistance, 1);
            entireRect.DOShakeAnchorPos(shakeTime, strength, vibrato, randomness, false, false, ShakeRandomnessMode.Harmonic);
        }

        bar.sizeDelta = new Vector2(newWidth, bar.sizeDelta.y);

        if (healthBarDecreaseSequence != null && healthBarDecreaseSequence.IsActive())
        {
            healthBarDecreaseSequence.Kill();
        }
        healthBarDecreaseSequence = DOTween.Sequence().OnKill(()=> healthBarDecreaseSequence = null);
        healthBarDecreaseSequence.PrependInterval(shakeTime * 2);
        healthBarDecreaseSequence.Append(changeBar.DOSizeDelta(new Vector2(newWidth, bar.sizeDelta.y), decreaseBarTime, false).SetEase(Ease.OutCubic));
        healthBarDecreaseSequence.Play();
    }
}
