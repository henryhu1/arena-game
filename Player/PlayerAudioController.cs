using System.Collections;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    [SerializeField] private float baseFootstepAudioBuffer = 0.4f;
    [SerializeField] private float sprintFootstepAudioBuffer = 0.3f;
    [SerializeField] private float backwardFootstepAudioBuffer = 0.3f;
    [SerializeField] private float sidewayFootstepAudioBuffer = 0.3f;

    [SerializeField] private AudioEffectSO footstepSound;

    [SerializeField] private AudioSource audioSource;

    [Header("Events")]
    [SerializeField] private Vector2EventChannelSO onPlayerMove;

    private Coroutine footstepAudioPlayer;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    private void Awake()
    {
        audioSource.clip = footstepSound.GetRandomClip();
    }

    private void OnEnable()
    {
        onPlayerMove.OnTwoDimensionEventRaised += PlayerMoveSound;
    }

    private void OnDisable()
    {
        onPlayerMove.OnTwoDimensionEventRaised -= PlayerMoveSound;
    }

    private void PlayerMoveSound(Vector2 movement)
    {
        if (movement == Vector2.zero || footstepAudioPlayer != null)
        {
            StopCoroutine(footstepAudioPlayer);
        }
        footstepAudioPlayer = StartCoroutine(PlayFootstepAudioEffect(movement));
    }

    private IEnumerator PlayFootstepAudioEffect(Vector2 movement)
    {
        while (manager.movementController.IsMoving())
        {
            if (!manager.IsControllerGrounded())
            {
                continue;
            }

            audioSource.Play();

            if (movement.x == 0 && movement.y > 0)
            {
                if (manager.movementController.GetIsSprinting())
                {
                    yield return new WaitForSeconds(sprintFootstepAudioBuffer);
                }
                else
                {
                    yield return new WaitForSeconds(baseFootstepAudioBuffer);
                }
            }
            else if (movement.x == 0 && movement.y < 0)
            {
                yield return new WaitForSeconds(backwardFootstepAudioBuffer);
            }
            else if (movement.y == 0)
            {
                yield return new WaitForSeconds(sidewayFootstepAudioBuffer);
            }
            else
            {
                yield return new WaitForSeconds(baseFootstepAudioBuffer);
            }
        }
    }
}
