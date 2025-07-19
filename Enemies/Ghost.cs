using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.VisualScripting;

public enum GhostAnimationState { Idle,Walk,Attack,Damage }
public class Ghost: MonoBehaviour, IKnockBackable, IEnemyAI
{
    Transform player;

    public NavMeshAgent Agent { get; private set; }
    public Rigidbody Rb { get; private set; }
    public float KnockbackTime { get; private set; }
    public float KnockbackDistance { get; private set; }

    public GameObject body;
    public GhostAnimationState currentState; 
    public Animator animator;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Rb = GetComponent<Rigidbody>();
        KnockbackTime = 1f;
        KnockbackDistance = 30f;
    }

    void Start()
    {
        player = PlayerManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentState = GhostAnimationState.Attack;
        }
    }

    void Update()
    {
        if (Agent.enabled)
        {
            Agent.SetDestination(player.position);
            if (currentState == GhostAnimationState.Idle)
            {
                currentState = GhostAnimationState.Walk;
            }
        }

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);

        switch (currentState)
        {
            case GhostAnimationState.Idle:

                if (animatorState.IsName("Idle")) return;
                StopAgent();
                break;

            case GhostAnimationState.Walk:

                if (animatorState.IsName("move")) return;

                StartAgent();
                break;

            case GhostAnimationState.Attack:

                if (animatorState.IsName("attack")) return;
                StopAgent();
                animator.Play("attack");
                break;

            case GhostAnimationState.Damage:

                if (animatorState.IsName("surprised")) return;

                StopAgent();
                animator.Play("surprised");
                break;

        }

        if (currentState != GhostAnimationState.Walk)
        {
            currentState = GhostAnimationState.Walk;
        }
    }

    public void StartAgent()
    {
        Agent.isStopped = false;
        Agent.updateRotation = true;
    }

    public void StopAgent()
    {
        Agent.isStopped = true;
        Agent.updateRotation = false;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        currentState = GhostAnimationState.Damage;
        StopAgent();
        Agent.enabled = false;
        transform.position = transform.position + direction * KnockbackDistance;
        StartCoroutine(KnockbackRoutine(direction, force));
    }

    public IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        float time = 0;
        Vector3 originalPos = transform.position;
        Vector3 endingPos = transform.position + direction * KnockbackDistance;

        while (time < KnockbackTime) // TODO: formula for knockback time?
        {
            //transform.position = Vector3.Lerp(originalPos, endingPos, time / KnockbackTime);
            time += Time.deltaTime;
            yield return null;
        }
        Agent.enabled = true;
    }
}