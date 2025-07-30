using UnityEngine;

public enum GhostAnimationState { Idle,Walk,Attack,Damage }
public class Ghost : EnemyControllerBase
{
    public GameObject body;
    public GhostAnimationState currentState;
    public Animator animator;

    protected override bool ShouldAttack()
    {
        return false;
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
        currentState = GhostAnimationState.Damage;
    }

    public override void WarpAgent(Vector3 pos, float distanceRange)
    {
        ai.WarpAgent(pos, distanceRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentState = GhostAnimationState.Attack;
        }
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

        }

        if (currentState != GhostAnimationState.Walk)
        {
            currentState = GhostAnimationState.Walk;
        }
    }
}