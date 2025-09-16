using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponName : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO onWeaponGet;
    [SerializeField] private WeaponEventChannelSO onWeaponDrop;

    private Color originalBackgroundColor;
    private Color transparentBackgroundColor;
    private Color originalTextColor;
    private Color transparentTextColor;

    [Header("Animation")]
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float fadeOutTime = 0.5f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        originalBackgroundColor = backgroundImage.color;
        transparentBackgroundColor = new Color(originalBackgroundColor.r, originalBackgroundColor.g, originalBackgroundColor.b, 0);
        originalTextColor = text.color;
        transparentTextColor = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 0);
    }

    private void Start()
    {
        backgroundImage.color = transparentBackgroundColor;
        text.color = transparentTextColor;
    }

    private void OnEnable()
    {
        onWeaponGet.OnWeaponEvent += DisplayWeaponName;
        onWeaponDrop.OnWeaponEvent += StopDisplayingWeaponName;
    }

    private void OnDisable()
    {
        onWeaponGet.OnWeaponEvent -= DisplayWeaponName;
        onWeaponDrop.OnWeaponEvent -= StopDisplayingWeaponName;
    }

    private void DisplayWeaponName(Weapon weapon)
    {
        StopAnimation();
        fadeCoroutine = StartCoroutine(FadeInOutWeaponName(weapon.GetWeaponData().weaponName));
    }

    private void StopDisplayingWeaponName(Weapon weapon)
    {
        StopAnimation();
    }

    private void StopAnimation()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            backgroundImage.color = transparentBackgroundColor;
            text.color = transparentTextColor;
        }
    }

    private IEnumerator FadeInOutWeaponName(string weaponName)
    {
        text.text = weaponName;

        backgroundImage.DOColor(originalBackgroundColor, fadeInTime);
        text.DOColor(originalTextColor, fadeInTime);

        yield return new WaitForSeconds(fadeInTime + displayTime);

        backgroundImage.DOColor(transparentBackgroundColor, fadeOutTime);
        text.DOColor(transparentTextColor, fadeOutTime);
    }
}
