using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Void Event Channel")]
public class VoidEventChannelSO : Vector3EventChannelSO
{
    public UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }

    public override void RaiseEvent(Vector3 pos)
    {
        OnEventRaised?.Invoke();
        base.RaiseEvent(pos);
    }
}
