using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour, IKnockBackable, IEnemyAI
{
    Transform player;

    public NavMeshAgent Agent { get; private set; }
    public Rigidbody Rb { get; private set; }
    public float KnockbackTime { get; private set; }
    public float KnockbackDistance { get; private set; }

    public GameObject body;
    public SlimeAnimationState currentState; 
    public Animator animator;

    //public Transform[] waypoints;
    //private int m_CurrentWaypointIndex;

    public Face faces;
    private Material faceMaterial;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Rb = GetComponent<Rigidbody>();
        KnockbackTime = 2.5f;
        KnockbackDistance = 0.0f;
    }

    void Start()
    {
        faceMaterial = body.GetComponent<Renderer>().materials[1];
        player = PlayerManager.Instance;
    }

    //public void WalkToNextDestination()
    //{
    //    currentState = SlimeAnimationState.Walk;
    //    m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
    //    Agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    //    SetFace(faces.WalkFace);
    //}

    //public void CancelGoNextDestination() => CancelInvoke(nameof(WalkToNextDestination));

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentState = SlimeAnimationState.Attack;
        }
    }

    void Update()
    {
        if (Agent.enabled)
        {
            Agent.SetDestination(player.position);
            if (currentState == SlimeAnimationState.Idle)
            {
                currentState = SlimeAnimationState.Walk;
            }
        }

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);

        switch (currentState)
        {
            case SlimeAnimationState.Idle:

                if (animatorState.IsName("Idle")) return;
                StopAgent();
                SetFace(faces.Idleface);
                break;

            case SlimeAnimationState.Walk:

                if (animatorState.IsName("Walk")) return;

                StartAgent();
                SetFace(faces.WalkFace);

                break;

            case SlimeAnimationState.Jump:

                if (animatorState.IsName("Jump")) return;

                StopAgent();
                SetFace(faces.jumpFace);
                animator.Play("Jump");

                break;

            case SlimeAnimationState.Attack:

                if (animatorState.IsName("Attack")) return;
                StopAgent();
                SetFace(faces.attackFace);
                animator.Play("Attack");

                break;
            case SlimeAnimationState.Damage:

                // Do nothing when animtion is playing
                if (animatorState.IsName("Damage0")
                     || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1")
                     || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2")) return;

                StopAgent();
                animator.Play("Damage2");
                SetFace(faces.damageFace);
                break;

        }
    }

    public void StartAgent()
    {
        Agent.isStopped = false;
        Agent.updateRotation = true;
        // set Speed parameter synchronized with agent root motion moverment
        animator.SetFloat("Speed", Agent.velocity.magnitude);
    }

    public void StopAgent()
    {
        Agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        Agent.updateRotation = false;
    }

    // Animation Event
    public void AlertObservers(string message)
    {
        if (message.Equals("AnimationDamageEnded"))
        {
            currentState = SlimeAnimationState.Walk;
        }

        if (message.Equals("AnimationAttackEnded"))
        {
            Debug.Log("animation attack ended");
            currentState = SlimeAnimationState.Walk;
        }

        if (message.Equals("AnimationJumpEnded"))
        {
            currentState = SlimeAnimationState.Walk;
        }
    }

    void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = Agent.nextPosition.y;
        transform.position = position;
        Agent.nextPosition = transform.position;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        currentState = SlimeAnimationState.Damage;
        //StartCoroutine(KnockbackRoutine(direction, force));
    }

    public IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        currentState = SlimeAnimationState.Damage;

        Agent.enabled = false;
        Rb.isKinematic = false;

        Rb.AddForce(direction.normalized * force, ForceMode.Impulse);

        yield return new WaitForSeconds(KnockbackTime); // TODO: formula for knockback time?

        Rb.isKinematic = true;
        Agent.enabled = true;
    }
}
