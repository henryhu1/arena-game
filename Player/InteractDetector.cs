using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private PlayerInteractHandler interactHandler;

    private void Awake()
    {
        interactHandler = GetComponentInParent<PlayerInteractHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var item))
        {
            interactHandler.AddToNearbyInteractables(item, other.transform.position);

            item.SetInteractor(interactHandler);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var item))
        {
            interactHandler.RemoveFromNearbyInteractables(item);
        }
    }
}