using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntEventChannelSO", menuName = "Events/Int")]
public class IntEventChannelSO : ScriptableObject
{
    public UnityAction<int> OnEventRaised;

    public void RaiseEvent(int integer)
    {
        OnEventRaised?.Invoke(integer);
    }
}
