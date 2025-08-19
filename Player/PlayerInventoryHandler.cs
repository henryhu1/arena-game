using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private CollectableItem holdingItem;
    
    [Header("Events")]
    [SerializeField] private CollectableItemEventChannelSO collectItemEvent;

    void Start()
    {
        collectItemEvent.OnCollectItemEvent += HoldItem;
    }

    void OnDestroy()
    {
        collectItemEvent.OnCollectItemEvent -= HoldItem;
    }

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void HoldItem(CollectableItem item) // TODO: use data structure to store items
    {
        if (item is Weapon weaponItem)
        {
            holdingItem = item;
        }
    }

    public bool IsHoldingWeapon()
    {
        return holdingItem is Weapon;
    }
}