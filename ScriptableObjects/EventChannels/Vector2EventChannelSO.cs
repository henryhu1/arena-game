using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Vector2")]
public class Vector2EventChannelSO : ScriptableObject
{
    public UnityAction<Vector2> OnTwoDimensionEventRaised;

    public virtual void RaiseEvent(Vector3 pos)
    {
        OnTwoDimensionEventRaised?.Invoke(pos);
    }
}
