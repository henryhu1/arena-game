using UnityEngine;

public class StartCanvas : MonoBehaviour
{
    [SerializeField] private AudioEffectSO buttonHoverEffect;
    [SerializeField] private AudioEffectSO buttonClickEffect;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHoverSound()
    {
        audioSource.PlayOneShot(buttonHoverEffect.GetRandomClip());
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(buttonClickEffect.GetRandomClip());
    }
}
