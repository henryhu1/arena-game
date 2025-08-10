using UnityEngine;

public enum CustomSlimeAnimationState { Idle,Walk,Attack,Damage,Death }
public class Slime : EnemyControllerBase
{
    public GameObject body;
    public CustomSlimeAnimationState currentState; 
    public Animator animator;

    public Face faces;
    private Material faceMaterial;

    void Start()
    {
        faceMaterial = body.GetComponent<Renderer>().materials[1];
    }

    public override void RestartAgent()
    {
        ai.EnableAgent();
    }

    public override void DisableAgent()
    {
        ai.DisableAgent();
    }

    public override void SetDamageState()
    {
        currentState = CustomSlimeAnimationState.Damage;
    }

    public override void SetAttackState()
    {
        currentState = CustomSlimeAnimationState.Attack;
    }

    public override bool CanAttack()
    {
        return currentState != CustomSlimeAnimationState.Damage && currentState != CustomSlimeAnimationState.Death;
    }

    public override void WarpAgent(Vector3 pos)
    {
        ai.WarpAgent(pos);
    }

    public override void HandleDeath()
    {
        currentState = CustomSlimeAnimationState.Death;
        base.HandleDeath();
    }

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player") && currentState != CustomSlimeAnimationState.Damage && currentState != CustomSlimeAnimationState.Death)
    //     {
    //         currentState = CustomSlimeAnimationState.Attack;
    //     }
    // }

    private void OnEnable()
    {
        currentState = CustomSlimeAnimationState.Idle;
    }

    protected override void Update()
    {
        base.Update();

        if (ai.IsAgentEnabled())
        {
            if (currentState == CustomSlimeAnimationState.Idle)
            {
                currentState = CustomSlimeAnimationState.Walk;
                animator.Play("Locomotion", 0, Random.value);
            }
        }

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);

        switch (currentState)
        {
            case CustomSlimeAnimationState.Idle:
                if (animatorState.IsName("Idle")) return;

                ai.StopAgent();

                animator.SetFloat("Speed", 0);

                SetFace(faces.Idleface);
                break;

            case CustomSlimeAnimationState.Walk:
                if (animatorState.IsName("Walk") || !ai.IsAgentEnabled()) return;

                ai.StartAgent();

                // set Speed parameter synchronized with agent root motion moverment
                animator.SetFloat("Speed", ai.GetVelocity());

                SetFace(faces.WalkFace);
                break;

            case CustomSlimeAnimationState.Attack:
                if (animatorState.IsName("Attack")) return;

                ai.StopAgent();

                animator.SetFloat("Speed", 0);
                animator.Play("Attack");

                SetFace(faces.attackFace);
                break;

            case CustomSlimeAnimationState.Damage:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1")) return;

                ai.StopAgent();

                animator.SetFloat("Speed", 0);
                animator.Play("Damage1");

                SetFace(faces.damageFace);
                break;

            case CustomSlimeAnimationState.Death:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2")) return;

                ai.StopAgent();

                animator.SetFloat("Speed", 0);
                animator.Play("Damage2");

                SetFace(faces.damageFace);
                break;
        }
    }

    // Animation Event
    public void AlertObservers(string message)
    {
        if (message.Equals("AnimationDamageEnded"))
        {
            if (!health.GetIsDead())
            {
                currentState = CustomSlimeAnimationState.Walk;
            }
        }

        if (message.Equals("AnimationAttackEnded"))
        {
            currentState = CustomSlimeAnimationState.Walk;
        }

        if (message.Equals("AnimationJumpEnded"))
        {
            currentState = CustomSlimeAnimationState.Walk;
        }
    }

    void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = ai.GetAgentY();
        transform.position = position;
        ai.SetAgentNextPosition(transform.position);
    }
}
