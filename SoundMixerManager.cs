using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance;

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }


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

    public float GetMasterVolume()
    {
        return GetVolume("masterVolume");
    }

    public float GetSoundFXVolume()
    {
        return GetVolume("soundFXVolume");
    }

    public float GetMusicVolume()
    {
        return GetVolume("musicVolume");
    }

    private float GetVolume(string name)
    {
        audioMixer.GetFloat(name, out float value);
        return Mathf.Pow(10, value / 20);
    }
}
