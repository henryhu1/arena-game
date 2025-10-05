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

        mainVolume.onValueChanged.AddListener(ChangeMainVolume);
        musicVolume.onValueChanged.AddListener(ChangeMusicVolume);
        soundFXVolume.onValueChanged.AddListener(ChangeSoundFXVolume);
    }

    private void OnDisable()
    {
        mainVolume.onValueChanged.RemoveListener(ChangeMainVolume);
        musicVolume.onValueChanged.RemoveListener(ChangeMusicVolume);
        soundFXVolume.onValueChanged.RemoveListener(ChangeSoundFXVolume);
    }

    private void ChangeMainVolume(float vol) { SoundMixerManager.Instance.SetMasterVolume(vol); }
    private void ChangeMusicVolume(float vol) { SoundMixerManager.Instance.SetMusicVolume(vol); }
    private void ChangeSoundFXVolume(float vol) { SoundMixerManager.Instance.SetSoundFXVolume(vol); }

    public void ChangeLocale(Locale locale) { LocalizationSettings.SelectedLocale = locale; }
}