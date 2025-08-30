using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Vector3 Event Channel")]
public class Vector3EventChannelSO : ScriptableObject
{
    public UnityAction<Vector3> OnPositionEventRaised;

    public virtual void RaiseEvent(Vector3 pos)
    {
        OnPositionEventRaised?.Invoke(pos);
    }
}
