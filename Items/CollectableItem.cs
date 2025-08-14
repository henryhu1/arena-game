using UnityEngine;

// TODO: add more subclasses
public class CollectableItem : Interactable
{
    public override void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerManager playerManager))
        {
            playerManager.interactHandler.RemoveFromNearbyInteractables(this);
            playerManager.inventoryHandler.HoldItem(this);
        }
    }
}
