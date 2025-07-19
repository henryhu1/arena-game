using UnityEngine;
using UnityEngine.AI;

public interface IEnemyAI
{
    NavMeshAgent Agent { get; }

    void StartAgent();
    void StopAgent();
}
