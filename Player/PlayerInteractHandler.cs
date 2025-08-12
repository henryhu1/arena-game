using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private HashSet<Interactable> nearbyInteractions = new();

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void AddToNearbyInteractables(Interactable item)
    {
        nearbyInteractions.Add(item);
    }

    public void RemoveFromNearbyInteractables(Interactable item)
    {
        nearbyInteractions.Remove(item);
    }

    public void InteractWithClosestInteraction()
    {
        if (nearbyInteractions.Count == 0) return;
        Interactable closest = GetClosestInteractable();
        closest.Interact(gameObject);
    }

    private Interactable GetClosestInteractable()
    {
        Interactable closest = null;
        float minDist = float.MaxValue;

        Vector3 playerPos = transform.position;

        foreach (var item in nearbyInteractions)
        {
            float dist = Vector3.SqrMagnitude(item.transform.position - playerPos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = item;
            }
        }

        return closest;
    }
}
