using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private CollectableItem heldItem;

    private WeaponData heldWeaponData;

    [Header("Model")]
    [SerializeField] private Transform rightGripPoint;
    [SerializeField] private Transform leftGripPoint;
    [SerializeField] private GameObject heldArrow;
    
    [Header("Events")]
    [SerializeField] private CollectableItemEventChannelSO collectItemEvent;
    [SerializeField] private WeaponEventChannelSO getWeaponEvent;
    [SerializeField] private WeaponEventChannelSO dropWeaponEvent;

    private void Start()
    {
        heldArrow.SetActive(false);
    }

    private void OnEnable()
    {
        collectItemEvent.OnCollectItemEvent += HoldItem;
        getWeaponEvent.OnWeaponEvent += EquipWeapon;
    }

    private void OnDisable()
    {
        collectItemEvent.OnCollectItemEvent -= HoldItem;
        getWeaponEvent.OnWeaponEvent -= EquipWeapon;
    }

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    private void HoldItem(CollectableItem item) // TODO: use data structure to store items
    {
        if (item is Weapon weaponItem)
        {
            // heldItem = item;
        }
    }

    private void EquipWeapon(Weapon weapon)
    {
        if (heldItem)
        {
            DropItem();
        }

        WeaponData weaponData = weapon.GetWeaponData();

        Transform handToHold = rightGripPoint;
        if (weaponData.heldByHand == WeaponData.HeldByHand.LEFT)
        {
            handToHold = leftGripPoint;
        }

        weapon.transform.SetParent(handToHold);
        weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(weaponData.eulerRotation));
        heldArrow.SetActive(weaponData.IsWeaponOfType(AttackType.BOW));
        weapon.StopPhysics();

        heldItem = weapon;
        heldWeaponData = weapon.GetWeaponData();
        manager.attackController.UseWeapon(weapon);
    }

    public WeaponData GetHeldWeaponData() { return heldWeaponData; }

    public void DropItem()
    {
        if (heldItem)
        {
            heldItem.SetInteractable();
        }
        if (heldItem is Weapon heldWeapon)
        {
            heldWeapon.transform.SetParent(null);
            heldWeapon.StartPhysics();
            dropWeaponEvent.RaiseEvent(heldWeapon);
            heldArrow.SetActive(false);
        }

        heldItem = null;
        heldWeaponData = null;
    }

    public bool IsHoldingWeapon() { return heldItem is Weapon; }
}