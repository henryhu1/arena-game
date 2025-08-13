using UnityEngine;

public class BoundsLimiter : MonoBehaviour
{
    [SerializeField] private BoundsSO worldBounds;
    [SerializeField] private bool clampPosition = true;

    void LateUpdate()
    {
        if (worldBounds == null) return;

        if (clampPosition)
        {
            transform.position = worldBounds.ClampPosition(transform.position);
        }
        else if (!worldBounds.IsInsideBounds(transform.position))
        {
            // You could trigger events or logic here if the object leaves bounds
            Debug.Log($"{name} is outside the world bounds!");
        }
    }
}
