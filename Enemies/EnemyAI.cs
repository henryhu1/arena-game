using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IEnemyComponent
{
    private EnemyStats stats;
    private EnemyControllerBase controllerBase;

    private NavMeshAgent agent;
    private Transform player;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.moveSpeed;

        player = PlayerManager.Instance;
    }

    private void Update()
    {
        if (agent.enabled && player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    public bool IsAgentEnabled() { return agent.enabled; }

    public void DisableAgent() { agent.enabled = false; }
    public void EnableAgent() { agent.enabled = true; }

    public float GetVelocity() { return agent.velocity.magnitude; }

    public float GetAgentY() { return agent.nextPosition.y; }

    public void SetAgentNextPosition(Vector3 pos) { agent.nextPosition = pos; }

    public void WarpAgent(Vector3 pos, float distanceRange)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, distanceRange, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning("Could not find NavMesh at the given XZ coordinates.");
        }
    }

    public void StartAgent()
    {
        if (!agent.enabled) return;
        agent.isStopped = false;
        agent.updateRotation = true;
    }

    public void StopAgent()
    {
        if (!agent.enabled) return;
        agent.isStopped = true;
        agent.updateRotation = false;
    }
}