using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour, IEnemyAttackBehavior
{
    private EnemyStats stats;
    private EnemyControllerBase controllerBase;

    private bool isAttacking;
    private bool hasDealtDamage;

    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private int playerLayer;

    public void Initialize(EnemyControllerBase controllerBase, EnemyStats stats)
    {
        this.controllerBase = controllerBase;
        this.stats = stats;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            player.TryGetComponent(out playerHealth);
        }
        playerLayer = LayerMask.GetMask("Player"); // Ensure player is on "Player" layer
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= stats.AttackRange() &&
            !isAttacking &&
            controllerBase.CanAttack())
        {
            StartCoroutine(PerformAttack());
        }
    }

    public IEnumerator PerformAttack()
    {
        isAttacking = true;
        hasDealtDamage = false;
        controllerBase.SetAttackState();

        yield return new WaitForSeconds(stats.AttackStart());

        // Damage window starts
        float damageWindow = stats.AttackEnd() - stats.AttackStart();
        float elapsed = 0f;

        while (elapsed < damageWindow)
        {
            if (!hasDealtDamage)
            {
                Vector3 boxCenter = transform.position + (transform.forward + Vector3.up) * (stats.AttackRange() / 2);
                if (Physics.CheckBox(boxCenter, Vector3.one * stats.AttackRange(), transform.rotation, playerLayer))
                {
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(stats.Damage());
                        hasDealtDamage = true;
                    }
                    break;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        isAttacking = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (stats != null)
        {
            Gizmos.color = Color.red;
            Vector3 boxCenter = transform.position + (transform.forward + Vector3.up) * (stats.AttackRange() / 2);
            Gizmos.DrawWireCube(boxCenter, Vector3.one * stats.AttackRange());
        }
    }
#endif
}