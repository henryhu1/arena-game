using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform content;

    [Header("Volume Sliders")]
    [SerializeField] private Slider mainVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider soundFXVolume;

    private void OnEnable()
    {
        mainVolume.value = SoundMixerManager.Instance.GetMasterVolume();
        musicVolume.value = SoundMixerManager.Instance.GetMusicVolume();
        soundFXVolume.value = SoundMixerManager.Instance.GetSoundFXVolume();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    public void ChangeLocale(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;
    }
}