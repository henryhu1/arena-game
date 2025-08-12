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
        if (other.TryGetComponent<Interactable>(out var item))
        {
            interactHandler.AddToNearbyInteractables(item);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Interactable>(out var item))
        {
            interactHandler.RemoveFromNearbyInteractables(item);
        }
    }
}