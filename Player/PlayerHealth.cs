using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    const float k_MaxHealthPoints = 100;

    private float healthPoints;
    private bool isGettingDamaged;
    [SerializeField] private float damageStunTime = 0.3f;

    public VoidEventChannelSO OnTakeDamage;
    public VoidEventChannelSO OnFreeFromDamage;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    void Awake()
    {
        healthPoints = k_MaxHealthPoints;
    }

    public bool GetIsGettingDamaged() { return isGettingDamaged; }

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
            // TODO: player death
        }
        else
        {
            isGettingDamaged = true;
            OnTakeDamage.RaiseEvent();
            StartCoroutine(DamageImmunityBuffer());
        }
    }
}