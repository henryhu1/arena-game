using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider mainVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider soundFXVolume;

    private void OnEnable()
    {
        mainVolume.value = SoundMixerManager.Instance.GetMasterVolume();
        musicVolume.value = SoundMixerManager.Instance.GetMusicVolume();
        soundFXVolume.value = SoundMixerManager.Instance.GetSoundFXVolume();
    }
}