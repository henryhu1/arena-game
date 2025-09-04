using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        SetVolume("masterVolume", level);
    }

    public void SetSoundFXVolume(float level)
    {
        SetVolume("soundFXVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        SetVolume("musicVolume", level);
    }

    private void SetVolume(string name, float level)
    {
        audioMixer.SetFloat(name, Mathf.Log10(level) * 20f);
    }
}
