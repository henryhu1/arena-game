using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    private bool isGettingDamaged;
    private bool isDead;

    [SerializeField] private FloatVariable playerHealth;

    [SerializeField] private float damageStunTime = 0.3f;

    [Header("Events")]
    [SerializeField] private Vector3EventChannelSO OnTakeDamage;
    [SerializeField] private FloatEventChannelSO OnPlayerHealthChange;
    [SerializeField] private VoidEventChannelSO OnFreeFromDamage;
    [SerializeField] private VoidEventChannelSO OnDeath;
    [SerializeField] private VoidEventChannelSO onGameRestart;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }

    private void Start()
    {
        ResetPlayerHealth();
    }

    private void OnEnable()
    {
        onGameRestart.OnEventRaised += ResetPlayerHealth;
    }

    private void OnDisable()
    {
        onGameRestart.OnEventRaised -= ResetPlayerHealth;
    }

    public void ResetPlayerHealth()
    {
        playerHealth.ResetValue();
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

    public void TakeDamage(Vector3 contactPos, float damagePoints)
    {
        if (isGettingDamaged) return;

        OnTakeDamage.RaiseEvent(contactPos);

        playerHealth.SubtractFromValue(damagePoints);

        if (playerHealth.GetValue() <= 0)
        {
            OnDeath.RaiseEvent();
            isDead = true;
        }
        else
        {
            isGettingDamaged = true;
            StartCoroutine(DamageImmunityBuffer());
        }
    }
}