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
        if (interactor.TryGetComponent(out PlayerManager playerManager))
        {
            transform.SetParent(playerManager.gripPoint.transform);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            itemCollider.enabled = false;
            itemRigidbody.isKinematic = true;

            weaponGetEvent.RaiseEvent(data);
        }
    }
}