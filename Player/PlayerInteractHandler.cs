using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private Dictionary<IInteractable, Vector3> nearbyInteractions = new();

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void AddToNearbyInteractables(IInteractable item, Vector3 pos)
    {
        nearbyInteractions.Add(item, pos);
    }

    public void RemoveFromNearbyInteractables(IInteractable item)
    {
        nearbyInteractions.Remove(item);
    }

    public void InteractWithClosestInteraction()
    {
        if (nearbyInteractions.Count == 0) return;
        IInteractable closest = GetClosestInteractable();
        closest.Interact(gameObject);
    }

    private IInteractable GetClosestInteractable()
    {
        IInteractable closest = null;
        float minDist = float.MaxValue;

        Vector3 playerPos = transform.position;

        foreach (KeyValuePair<IInteractable, Vector3> interactable in nearbyInteractions)
        {
            float dist = Vector3.SqrMagnitude(interactable.Value - playerPos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = interactable.Key;
            }
        }

        return closest;
    }
}
