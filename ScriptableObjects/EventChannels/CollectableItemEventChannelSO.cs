using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CollectableItemEventChannelSO", menuName = "Events/Collectable Item Event Channel")]
public class CollectableItemEventChannelSO : ScriptableObject
{
    public UnityAction<CollectableItem> OnCollectItemEvent;

    public void RaiseEvent(CollectableItem item)
    {
        OnCollectItemEvent?.Invoke(item);
    }
}
