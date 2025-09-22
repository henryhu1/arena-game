using System.Collections;
using UnityEngine;

public class CollectableItem : MonoBehaviour, IInteractable
{
    [Header("Spawning")]
    public ItemSpawnStrategy spawnStrategy;

    [Header("Events")]
    [SerializeField] private CollectableItemEventChannelSO onCollectItem;
    [SerializeField] private VoidEventChannelSO onGameRestart;

    private PlayerInteractHandler playerInteractor;

    private ParticleSystem ps;

    private Coroutine playingParticle;

    const float k_playDuration = 3f;
    const float k_pauseDuration = 3f; // TODO: randomize?

    protected virtual void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        SetInteractable();
    }

    private void OnEnable()
    {
        SetInteractable();
        onGameRestart.OnEventRaised += Despawn;
    }

    private void OnDisable()
    {
        onGameRestart.OnEventRaised -= Despawn;
        SetNotInteractable();
    }

    private IEnumerator PlayParticle()
    {
        while (IsInteractable())
        {
            ps.Clear(true);
            ps.Play(true);

            yield return new WaitForSeconds(k_playDuration);

            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            yield return new WaitForSeconds(k_pauseDuration);
        }
    }

    public void SetInteractor(PlayerInteractHandler playerInteractor)
    {
        this.playerInteractor = playerInteractor;
    }

    public virtual void Interact(GameObject interactor)
    {
        if (!IsInteractable()) return;

        if (playingParticle != null)
        {
            StopCoroutine(playingParticle);
        }
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        onCollectItem.RaiseEvent(this);
    }

    public string GetInteractionText()
    {
        // TODO: localization
        return "Pick up";
    }

    public Vector3 GetScreenPos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    public bool IsInteractable()
    {
        return gameObject.layer == LayerMask.NameToLayer("Interactables");
    }

    // TODO: set layer safely
    protected void SetNotInteractable(string layer = "Default")
    {
        gameObject.layer = LayerMask.NameToLayer(layer);
    }

    public void SetInteractable()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactables");
        playingParticle = StartCoroutine(PlayParticle());
    }

    public void Despawn()
    {
        if (spawnStrategy != null)
        {
            // ItemSpawner.Instance.DespawnItem(spawnStrategy.itemPrefab);
            if (playerInteractor != null)
            {
                playerInteractor.RemoveFromNearbyInteractables(this);
            }
            ItemSpawner.Instance.DespawnItem(gameObject, spawnStrategy.itemPrefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
