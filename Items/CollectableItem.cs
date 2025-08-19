using UnityEngine;

// TODO: add more subclasses
public class CollectableItem : MonoBehaviour, IInteractable
{
    [Header("Events")]
    [SerializeField] private CollectableItemEventChannelSO collectItemEvent;

    public virtual void Interact(GameObject interactor)
    {
        if (!IsInteractable()) return;

        collectItemEvent.RaiseEvent(this);
    }

    protected bool IsInteractable()
    {
        return gameObject.layer == LayerMask.NameToLayer("Interactables");
    }

    // TODO: set layer safely
    protected void SetNotInteractable(string layer)
    {
        gameObject.layer = LayerMask.NameToLayer(layer);
    }

    protected void SetInteractable()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactables");
    }
}
