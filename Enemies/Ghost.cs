using UnityEngine;

public class Ghost : EnemyControllerBase
{
    protected override void Update()
    {
        if (ai.IsAgentEnabled())
        {
            if (currentState == EnemyAnimation.Idle)
            {
                currentState = EnemyAnimation.Walk;
            }
        }

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);

        switch (currentState)
        {
            case EnemyAnimation.Idle:
                if (animatorState.IsName("Idle")) return;
                break;

            case EnemyAnimation.Walk:
                if (animatorState.IsName("move")) return;
                break;

            case EnemyAnimation.Attack:
                if (animatorState.IsName("attack")) return;
                animator.Play("attack");
                break;

            case EnemyAnimation.Damage:
                if (animatorState.IsName("surprised")) return;
                animator.Play("surprised");
                break;

            case EnemyAnimation.Death:
                if (animatorState.IsName("dissolve")) return;
                animator.Play("dissolve");
                break;

        }
    }
}