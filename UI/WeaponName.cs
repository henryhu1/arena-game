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
    [SerializeField] private VoidEventChannelSO onGameOver;

    private Color originalBackgroundColor;
    private Color transparentBackgroundColor;
    private Color originalTextColor;
    private Color transparentTextColor;

    [Header("Animation")]
    [SerializeField] private float fadeInTime = 0.5f;

    private RectTransform rectTransform;
    private Tween backgroundTween;
    private Tween textTween;
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
        onGameOver.OnEventRaised += StopDisplayingWeaponName;
    }

    private void OnDisable()
    {
        onWeaponGet.OnWeaponEvent -= DisplayWeaponName;
        onWeaponDrop.OnWeaponEvent -= StopDisplayingWeaponName;
        onGameOver.OnEventRaised -= StopDisplayingWeaponName;
    }

    private void DisplayWeaponName(Weapon weapon)
    {
        StopDisplayingWeaponName();

        string localizedName = weaponNamesTable.GetEntry(weapon.GetWeaponData().weaponKey).GetLocalizedString();
        text.text = localizedName;
        backgroundImage.enabled = true;
        text.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        FadeInWeaponName();
    }

    private void StopDisplayingWeaponName(Weapon _)
    {
        StopDisplayingWeaponName();
    }

    private void StopDisplayingWeaponName()
    {
        backgroundTween?.Kill();
        backgroundImage.color = transparentBackgroundColor;
        backgroundImage.enabled = false;
        textTween?.Kill();
        text.color = transparentTextColor;
        text.enabled = false;
    }

    private void FadeInWeaponName()
    {
        backgroundTween = backgroundImage.DOColor(originalBackgroundColor, fadeInTime);
        textTween = text.DOColor(originalTextColor, fadeInTime);
    }
}
