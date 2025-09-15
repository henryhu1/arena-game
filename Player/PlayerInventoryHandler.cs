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
            heldItem = item;
        }
    }

    private void EquipWeapon(Weapon weapon)
    {
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

        manager.attackController.UseWeapon(weapon);
        heldWeaponData = weapon.GetWeaponData();
    }

    public WeaponData GetHeldWeaponData() { return heldWeaponData; }

    public bool IsHoldingWeapon() { return heldItem is Weapon; }
}