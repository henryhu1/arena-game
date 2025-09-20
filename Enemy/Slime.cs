using UnityEngine;

public class Slime : EnemyControllerBase
{
    [Header("Slime Specific")]
    public GameObject body;

    public Face faces;
    private Material faceMaterial;

    void Start()
    {
        faceMaterial = body.GetComponent<Renderer>().materials[1];
    }

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    protected override void Update()
    {
        base.Update();

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);

        switch (currentState)
        {
            case EnemyAnimation.Idle:
                if (animatorState.IsName("Idle")) return;
                animator.SetFloat("Speed", 0);

                SetFace(faces.Idleface);
                break;

            case EnemyAnimation.Walk:
                if (animatorState.IsName("Walk") || !ai.IsAgentEnabled()) return;
                // set Speed parameter synchronized with agent root motion moverment
                animator.SetFloat("Speed", ai.GetVelocity());

                SetFace(faces.WalkFace);
                break;

            case EnemyAnimation.Attack:
                if (animatorState.IsName("Attack")) return;
                animator.SetFloat("Speed", 0);

                SetFace(faces.attackFace);
                break;

            case EnemyAnimation.Damage:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1")) return;
                animator.SetFloat("Speed", 0);

                SetFace(faces.damageFace);
                break;

            case EnemyAnimation.Death:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2")) return;
                animator.SetFloat("Speed", 0);

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
                currentState = EnemyAnimation.Walk;
            }
        }

        if (message.Equals("AnimationAttackEnded"))
        {
            currentState = EnemyAnimation.Walk;
        }

        if (message.Equals("AnimationJumpEnded"))
        {
            currentState = EnemyAnimation.Walk;
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
