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
    [SerializeField] private Vector3EventChannelSO OnTakeDamage;
    [SerializeField] private FloatEventChannelSO OnPlayerHealthChange;
    [SerializeField] private VoidEventChannelSO OnFreeFromDamage;
    [SerializeField] private VoidEventChannelSO OnDeath;
    [SerializeField] private VoidEventChannelSO onGameRestart;

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
        healthPoints = k_MaxHealthPoints;
        isDead = false;
        OnPlayerHealthChange.RaiseEvent(healthPoints);
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

        healthPoints = Mathf.Max(healthPoints - damagePoints, 0);
        OnPlayerHealthChange.RaiseEvent(healthPoints);

        if (healthPoints <= 0)
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