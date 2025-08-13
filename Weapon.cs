using UnityEngine;

public class Weapon : CollectableItem
{
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
            // playerManager.Collect(this); TODO: collectable item effects
            transform.SetParent(playerManager.gripPoint.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            itemCollider.enabled = false;
            itemRigidbody.isKinematic = true;
        }
    }
}