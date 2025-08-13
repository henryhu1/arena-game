using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private CollectableItem holdingItem;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void HoldItem(CollectableItem item)
    {
        holdingItem = item;
    }

    public bool IsHoldingWeapon()
    {
        return holdingItem is Weapon;
    }
}