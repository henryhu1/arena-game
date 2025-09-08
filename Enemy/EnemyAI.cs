using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IEnemyComponent
{
    private EnemyStats stats;
    private EnemyControllerBase controllerBase;

    private NavMeshAgent agent;
    private Transform player;

    private bool shouldFollowPlayer;
    private float originalStoppingDistance;

    private Coroutine wanderingCoroutine;

    private const float k_wanderDistanceMin = 20f;
    private const float k_wanderDistanceMax = 70f;
    private const float k_wanderStoppingDistance = 10f;
    private const float k_wanderTimeMin = 2f;
    private const float k_wanderTimeMax = 8f;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onPlayerDeath;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.MoveSpeed();
        agent.stoppingDistance = stats.sizeMultiplier;
        originalStoppingDistance = agent.stoppingDistance;
    }

    private void Start()
    {
        player = PlayerManager.Instance.transform;
        shouldFollowPlayer = true;

        onPlayerDeath.OnEventRaised += WanderAgent;
    }

    private void OnDestroy()
    {
        onPlayerDeath.OnEventRaised -= WanderAgent;
    }

    private void Update()
    {
        if (!agent.enabled) return;

        if (shouldFollowPlayer)
        {
            if (wanderingCoroutine != null)
            {
                StopCoroutine(wanderingCoroutine);
            }
            agent.stoppingDistance = originalStoppingDistance;
            agent.SetDestination(player.position);
        }
        else
        {
            wanderingCoroutine ??= StartCoroutine(WanderAI());
        }
    }

    private void WanderAgent()
    {
        shouldFollowPlayer = false;
    }

    private IEnumerator WanderAI()
    {
        agent.stoppingDistance = k_wanderStoppingDistance;
        while (!shouldFollowPlayer)
        {
            float wanderDistance = Random.Range(k_wanderDistanceMin, k_wanderDistanceMax);
            Vector3 wanderToPosition = EnemyPositionUtils.GetRandomPositionInNavSphere(transform.position, wanderDistance);
            agent.SetDestination(wanderToPosition);

            float wanderTime = Random.Range(k_wanderTimeMin, k_wanderTimeMax);
            yield return new WaitForSeconds(wanderTime);
        }
    }

    public bool IsAgentEnabled() { return agent.enabled; }

    public void DisableAgent() { agent.enabled = false; }
    public void EnableAgent() { agent.enabled = true; }
    public bool IsFollowingPlayer() { return shouldFollowPlayer; }
    public bool IsDestinationReached() { return !agent.hasPath || agent.velocity.sqrMagnitude == 0; }
    public float GetVelocity() { return agent.velocity.magnitude; }

    public float GetAgentY() { return agent.nextPosition.y; }

    public void SetAgentNextPosition(Vector3 pos) { agent.nextPosition = pos; }

    public Vector3 WarpAgent(Vector3 pos)
    {
        Vector3 positionOnNavMesh = EnemyPositionUtils.GetPositionOnNavMesh(pos);
        if (positionOnNavMesh != Vector3.negativeInfinity && agent.Warp(positionOnNavMesh))
        {
            return positionOnNavMesh;
        }
        Debug.LogWarning("Could not find NavMesh at the given XZ coordinates.");
        return pos;
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

    public void ResetAgent(Vector3 pos)
    {
        agent.speed = stats.MoveSpeed();
        agent.stoppingDistance = stats.sizeMultiplier;
        EnableAgent();
        WarpAgent(pos);
    }
}