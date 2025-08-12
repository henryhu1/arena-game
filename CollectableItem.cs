using UnityEngine;

public class CollectableItem : Interactable
{
    public string itemName;

    public override void Interact(GameObject interactor)
    {
        Debug.Log($"Picked up {itemName}");
        if (interactor.TryGetComponent(out PlayerManager playerManager))
        {
            playerManager.interactHandler.RemoveFromNearbyInteractables(this);
            // playerManager.Collect(this); TODO: collectable item effects
        }
        Destroy(gameObject);
    }
}
