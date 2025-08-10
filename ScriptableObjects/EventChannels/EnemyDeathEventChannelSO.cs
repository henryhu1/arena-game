using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Enemy Death Event Channel")]
public class EnemyDeathEventChannelSO : ScriptableObject
{
    public UnityAction<EnemyControllerBase> OnEnemyDied;

    public void RaiseEvent(EnemyControllerBase enemy)
    {
        OnEnemyDied?.Invoke(enemy);
    }
}
