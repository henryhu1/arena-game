using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntEventChannelSO", menuName = "Events/Int Event Channel")]
public class IntEventChannelSO : ScriptableObject
{
    public UnityAction<int> OnEventRaised;

    public void RaiseEvent(int integer)
    {
        OnEventRaised?.Invoke(integer);
    }
}
