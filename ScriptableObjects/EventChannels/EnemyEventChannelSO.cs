using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Enemy")]
public class EnemyEventChannelSO : Vector3EventChannelSO
{
    public UnityAction<EnemyControllerBase> OnEnemyEvent;

    public void RaiseEvent(EnemyControllerBase enemy)
    {
        OnEnemyEvent?.Invoke(enemy);
        OnPositionEventRaised?.Invoke(enemy.transform.position);
    }
}
