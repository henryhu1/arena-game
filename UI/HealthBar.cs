using DG.Tweening;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform bar;
    private float originalWidth;
    private Tween healthBarAnimation;
    const float k_animationTime = 1f;

    [Header("Events")]
    [SerializeField] private FloatEventChannelSO onPlayerHealthChange;

    private void Awake()
    {
        originalWidth = bar.sizeDelta.x;
    }

    private void OnEnable()
    {
        onPlayerHealthChange.OnEventRaised += UpdateBarWidth;
    }

    private void OnDisable()
    {
        onPlayerHealthChange.OnEventRaised -= UpdateBarWidth;
    }

    private void UpdateBarWidth(float healthPoints)
    {
        float width = originalWidth * healthPoints / 100;
        healthBarAnimation = bar.DOSizeDelta(new Vector2(width, bar.sizeDelta.y), k_animationTime, false).SetEase(Ease.OutQuart);
    }
}
