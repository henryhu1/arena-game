using UnityEngine;

public class Consumable : CollectableItem
{
    // [Header("Values")]
    // [SerializeField] private WeaponData data;

    // [Header("Events")]
    // [SerializeField] private WeaponEventChannelSO weaponGetEvent;

    // private Collider itemCollider;
    // private Rigidbody itemRigidbody;

    // void Awake()
    // {
    //     itemCollider = GetComponent<Collider>();
    //     itemRigidbody = GetComponent<Rigidbody>();
    // }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        gameObject.SetActive(false);

        if (interactor.TryGetComponent(out PlayerManager playerManager))
        {
            // TODO: add to inventory, account for max amount
        }
    }
}

