using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCount : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private IntVariable arrowCount;

    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RawImage arrowImage;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO onPlayerWeaponChange;
    [SerializeField] private IntEventChannelSO onArrowCountChange;

    private Color originalBackgroundColor;
    private Color transparentBackgroundColor;
    private Color originalArrowColor;
    private Color originalTextColor;
    private Color transparentTextColor;
    private Color transparentArrowColor;

    [Header("Animation")]
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;

    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    private void Awake()
    {
        originalBackgroundColor = backgroundImage.color;
        transparentBackgroundColor = new Color(originalBackgroundColor.r, originalBackgroundColor.g, originalBackgroundColor.b, 0);
        originalArrowColor = arrowImage.color;
        transparentArrowColor = new Color(originalArrowColor.r, originalArrowColor.g, originalArrowColor.b, 0);
        originalTextColor = text.color;
        transparentTextColor = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 0);
        text.text = arrowCount.GetValue().ToString();
    }

    private void Start()
    {
        backgroundImage.color = transparentBackgroundColor;
        arrowImage.color = transparentArrowColor;
        text.color = transparentTextColor;
    }

    private void OnEnable()
    {
        onPlayerWeaponChange.OnWeaponEvent += ToggleAmmoDisplay;
        onArrowCountChange.OnEventRaised += ChangeArrowCount;
    }

    private void OnDisable()
    {
        onPlayerWeaponChange.OnWeaponEvent -= ToggleAmmoDisplay;
        onArrowCountChange.OnEventRaised -= ChangeArrowCount;
    }

    private void ToggleAmmoDisplay(Weapon weapon)
    {
        if (weapon == null || !weapon.GetWeaponData().IsWeaponOfType(AttackType.BOW))
        {
            StopDisplayingAmmo();
        }
        else
        {
            if (fadeInCoroutine != null)
            {
                StopCoroutine(fadeInCoroutine);
            }
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
            }
            fadeInCoroutine = StartCoroutine(FadeInAmmoDisplay());
        }
    }

    private void StopDisplayingAmmo()
    {
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }
        fadeOutCoroutine = StartCoroutine(FadeOutAmmoDisplay());
    }

    private void ChangeArrowCount(int count)
    {
        text.text = count.ToString();
    }

    private IEnumerator FadeInAmmoDisplay()
    {
        backgroundImage.DOColor(originalBackgroundColor, fadeInTime);
        arrowImage.DOColor(originalArrowColor, fadeInTime);
        text.DOColor(originalTextColor, fadeInTime);
        yield return new WaitForSeconds(fadeInTime);
        fadeInCoroutine = null;
    }

    private IEnumerator FadeOutAmmoDisplay()
    {
        backgroundImage.DOColor(transparentBackgroundColor, fadeOutTime);
        arrowImage.DOColor(transparentArrowColor, fadeOutTime);
        text.DOColor(transparentTextColor, fadeOutTime);
        yield return new WaitForSeconds(fadeOutTime);
        fadeOutCoroutine = null;
    }
}
