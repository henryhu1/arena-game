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

    public void SetInteractor(PlayerInteractHandler playerInteractor)
    {
        this.playerInteractor = playerInteractor;
    }

    public virtual void Interact(GameObject interactor)
    {
        if (!IsInteractable()) return;

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
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void SetInteractable()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactables");
        ps.Clear(true);
        ps.Play(true);
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
