using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    const float k_MaxHealthPoints = 100;

    private float healthPoints;
    private bool isGettingDamaged;
    private bool isDead;
    [SerializeField] private float damageStunTime = 0.3f;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO OnTakeDamage;
    [SerializeField] private VoidEventChannelSO OnFreeFromDamage;
    [SerializeField] private VoidEventChannelSO OnDeath;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    void Awake()
    {
        healthPoints = k_MaxHealthPoints;
    }

    private void Start()
    {
        ResetPlayerHealth();
    }

    public void ResetPlayerHealth()
    {
        healthPoints = k_MaxHealthPoints;
        isDead = false;
    }

    public bool GetIsGettingDamaged() { return isGettingDamaged; }
    public bool GetIsDead() { return isDead; }

    private IEnumerator DamageImmunityBuffer()
    {
        yield return new WaitForSeconds(damageStunTime);
        isGettingDamaged = false;
        OnFreeFromDamage.RaiseEvent();
    }

    public void TakeDamage(float damagePoints)
    {
        if (isGettingDamaged) return;

        healthPoints -= damagePoints;

        if (healthPoints <= 0)
        {
            OnDeath.RaiseEvent();
            isDead = true;
        }
        else
        {
            isGettingDamaged = true;
            OnTakeDamage.RaiseEvent();
            StartCoroutine(DamageImmunityBuffer());
        }
    }
}