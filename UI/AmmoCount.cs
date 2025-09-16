using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCount : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RawImage arrowImage;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO onWeaponGet;
    [SerializeField] private WeaponEventChannelSO onWeaponDrop;

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
    }

    private void Start()
    {
        backgroundImage.color = transparentBackgroundColor;
        arrowImage.color = transparentArrowColor;
        text.color = transparentTextColor;
    }

    private void OnEnable()
    {
        // TODO: maybe watch a weapon change event instead of get and drop
        onWeaponGet.OnWeaponEvent += ToggleAmmoDisplay;
        onWeaponDrop.OnWeaponEvent += DropWeaponHandler;
    }

    private void OnDisable()
    {
        onWeaponGet.OnWeaponEvent -= ToggleAmmoDisplay;
        onWeaponDrop.OnWeaponEvent -= DropWeaponHandler;
    }

    private void ToggleAmmoDisplay(Weapon weapon)
    {
        Debug.Log(weapon.GetWeaponData().attackType);
        if (weapon.GetWeaponData().IsWeaponOfType(AttackType.BOW))
        {
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
            }
            fadeInCoroutine ??= StartCoroutine(FadeInAmmoDisplay());
        }
        else if (!weapon.GetWeaponData().IsWeaponOfType(AttackType.BOW))
        {
            StopDisplayingAmmo();
        }
    }

    private void DropWeaponHandler(Weapon weapon)
    {
        if (weapon.GetWeaponData().IsWeaponOfType(AttackType.BOW))
        {
            StopDisplayingAmmo();
        }
    }

    private void StopDisplayingAmmo()
    {
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        fadeOutCoroutine ??= StartCoroutine(FadeOutAmmoDisplay());
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
