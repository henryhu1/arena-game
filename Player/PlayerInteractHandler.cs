using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private Dictionary<int, (IInteractable, Vector3)> nearbyInteractions = new();

    [Header("Events")]
    [SerializeField] private CollectableItemEventChannelSO collectItemEvent;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    void OnEnable()
    {
        collectItemEvent.OnCollectItemEvent += RemoveFromNearbyInteractables;
    }

    void OnDisable()
    {
        collectItemEvent.OnCollectItemEvent -= RemoveFromNearbyInteractables;
    }

    public void AddToNearbyInteractables(IInteractable item, int id, Vector3 pos)
    {
        if (!nearbyInteractions.ContainsKey(id))
        {
            nearbyInteractions.Add(id, (item, pos));
        }
    }

    public void RemoveFromNearbyInteractables(int id)
    {
        nearbyInteractions.Remove(id);
    }

    public void RemoveFromNearbyInteractables(CollectableItem item)
    {
        nearbyInteractions.Remove(item.gameObject.GetInstanceID());
    }

    public void InteractWithClosestInteraction()
    {
        if (nearbyInteractions.Count == 0) return;
        IInteractable closest = GetClosestInteractable();
        closest.Interact(gameObject);
    }

    public IInteractable GetClosestInteractable()
    {
        IInteractable closest = null;
        float minDist = float.MaxValue;

        Vector3 playerPos = transform.position;

        foreach (KeyValuePair<int, (IInteractable, Vector3)> interactable in nearbyInteractions)
        {
            float dist = Vector3.SqrMagnitude(interactable.Value.Item2 - playerPos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = interactable.Value.Item1;
            }
        }

        return closest;
    }
}
