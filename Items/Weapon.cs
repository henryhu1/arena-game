using UnityEngine;

public class Weapon : CollectableItem
{
    [Header("Values")]
    [SerializeField] private WeaponData data;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO weaponGetEvent;

    private Collider itemCollider;
    private Rigidbody itemRigidbody;

    void Awake()
    {
        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        // TODO: utilize the event to make transform changes
        if (interactor.TryGetComponent(out PlayerManager playerManager))
        {
            if (data.IsWeaponOfType(AttackType.BOW))
            {
                transform.SetParent(playerManager.leftGripPoint.transform);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(225, 0, 0));
                playerManager.heldArrow.SetActive(true);
            }
            else
            {
                transform.SetParent(playerManager.rightGripPoint.transform);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                playerManager.heldArrow.SetActive(false);
            }
            itemCollider.enabled = false;
            itemRigidbody.isKinematic = true;

            weaponGetEvent.RaiseEvent(data);
        }
    }
}
