using UnityEngine;

public enum GhostAnimationState { Idle,Walk,Attack,Damage,Death }
public class Ghost : EnemyControllerBase
{
    public GameObject body;
    public GhostAnimationState currentState;
    public Animator animator;

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
        currentState = GhostAnimationState.Damage;
    }

    public override void SetAttackState()
    {
        currentState = GhostAnimationState.Attack;
    }

    public override bool CanAttack()
    {
        return currentState != GhostAnimationState.Damage;
    }

    public override void WarpAgent(Vector3 pos)
    {
        ai.WarpAgent(pos);
    }

    public override void HandleDeath()
    {
        currentState = GhostAnimationState.Death;
        base.HandleDeath();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         currentState = GhostAnimationState.Attack;
    //     }
    // }

    private void OnEnable()
    {
        currentState = GhostAnimationState.Idle;
    }

    protected override void Update()
    {
        if (ai.IsAgentEnabled())
        {
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

                ai.StopAgent();
                break;

            case GhostAnimationState.Walk:
                if (animatorState.IsName("move")) return;

                ai.StartAgent();
                break;

            case GhostAnimationState.Attack:
                if (animatorState.IsName("attack")) return;

                ai.StopAgent();
                animator.Play("attack");
                break;

            case GhostAnimationState.Damage:
                if (animatorState.IsName("surprised")) return;

                ai.StopAgent();
                animator.Play("surprised");
                break;

            case GhostAnimationState.Death:
                if (animatorState.IsName("dissolve")) return;

                ai.StopAgent();
                animator.Play("dissolve");
                break;

        }

        if (currentState != GhostAnimationState.Walk)
        {
            currentState = GhostAnimationState.Walk;
        }
    }
}