using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Bool")]
public class BoolEventChannelSO : ScriptableObject
{
    public UnityAction<bool> OnEventRaised;

    public virtual void RaiseEvent(bool pos)
    {
        OnEventRaised?.Invoke(pos);
    }
}
