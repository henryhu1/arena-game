using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float value)
    {
        SetVolume("masterVolume", value);
    }

    public void SetSoundFXVolume(float value)
    {
        SetVolume("soundFXVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("musicVolume", value);
    }

    private void SetVolume(string name, float value)
    {
        audioMixer.SetFloat(name, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }
}
