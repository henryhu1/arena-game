using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Enemy Event Channel")]
public class EnemyEventChannelSO : ScriptableObject
{
    public UnityAction<EnemyControllerBase> OnEnemyEvent;

    public void RaiseEvent(EnemyControllerBase enemy)
    {
        OnEnemyEvent?.Invoke(enemy);
    }
}
