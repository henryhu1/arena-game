using UnityEngine;

// TODO: add more subclasses
public class CollectableItem : MonoBehaviour, IInteractable
{
    public virtual void Interact(GameObject interactor)
    {
        if (!IsInteractable()) return;

        if (interactor.TryGetComponent(out PlayerManager playerManager))
        {
            playerManager.interactHandler.RemoveFromNearbyInteractables(this);
            playerManager.inventoryHandler.HoldItem(this);
        }
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
