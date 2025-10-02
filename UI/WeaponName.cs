using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
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

    private RectTransform rectTransform;
    private Coroutine fadeCoroutine;
    private StringTable weaponNamesTable;

    private async void Awake()
    {
        originalBackgroundColor = backgroundImage.color;
        transparentBackgroundColor = new Color(originalBackgroundColor.r, originalBackgroundColor.g, originalBackgroundColor.b, 0);
        originalTextColor = text.color;
        transparentTextColor = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 0);

        weaponNamesTable = await LocalizationSettings.StringDatabase.GetTableAsync("WeaponNames").Task;
    }

    private void Start()
    {
        backgroundImage.color = transparentBackgroundColor;
        text.color = transparentTextColor;
        rectTransform = GetComponent<RectTransform>();
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
        string localizedName = weaponNamesTable.GetEntry(weapon.GetWeaponData().weaponKey).GetLocalizedString();
        text.text = localizedName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        fadeCoroutine = StartCoroutine(FadeInOutWeaponName());
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

    private IEnumerator FadeInOutWeaponName()
    {
        backgroundImage.DOColor(originalBackgroundColor, fadeInTime);
        text.DOColor(originalTextColor, fadeInTime);

        yield return new WaitForSeconds(fadeInTime + displayTime);

        backgroundImage.DOColor(transparentBackgroundColor, fadeOutTime);
        text.DOColor(transparentTextColor, fadeOutTime);
    }
}
