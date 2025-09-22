using UnityEngine;

// TODO: separate weapon script from collectable item?
public class Weapon : CollectableItem
{
    [Header("Values")]
    [SerializeField] private WeaponData data;
    [SerializeField] private float remainingUses = 10;

    [Header("Model")]
    [SerializeField] private Transform impactPoint;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO weaponGetEvent;

    private Collider itemCollider;
    private Rigidbody itemRigidbody;

    protected override void Awake()
    {
        base.Awake();

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();
    }

    public void StopPhysics()
    {
        itemCollider.enabled = false;
        itemRigidbody.isKinematic = true;
    }

    public void StartPhysics()
    {
        itemCollider.enabled = true;
        itemRigidbody.isKinematic = false;
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        weaponGetEvent.RaiseEvent(this);
        SetNotInteractable();
    }

    public void DecreaseUses()
    {
        remainingUses--;
    }

    public float GetRemainingUses() { return remainingUses; }

    public WeaponData GetWeaponData() { return data; }
    public Transform GetImpactPoint() { return impactPoint; }
}
