using UnityEngine;
using NaughtyAttributes;
using System;

public class BossStats : MonoBehaviour, IDamageable
{
    [Header("STATS")]
    [ReadOnly] public float health;
    [SerializeField] private float maxHealth;
    public float defense = 0f;

    [Header("REFERENCES")]
    [SerializeField] private BossUI bossUI;

    private bool isInvulnerable;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    public float MaxHealth => maxHealth;
    public bool IsDead => health <= 0f;
    public bool IsInvulnerable => isInvulnerable;

    void Start(){
        health = maxHealth;
        if(bossUI == null) bossUI = GetComponentInChildren<BossUI>();

        OnHealthChanged?.Invoke(health, maxHealth);
        if(bossUI != null) bossUI.UpdateHealth(health, maxHealth);
    }

    public void SetInvulnerable(bool value) => isInvulnerable = value;
    public bool TakeDamage(float amount){
        if(IsDead) return false;
        if(isInvulnerable) return false;

        health -= (amount - defense);
        AudioManager.Instance.PlaySFX("boss_take_hit");
        if(health < 0f) health = 0f;

        OnHealthChanged?.Invoke(health, maxHealth);
        if(bossUI != null) bossUI.UpdateHealth(health, maxHealth);

        if(health <= 0f) Die();
        return true;
    }

    void Die(){
        AudioManager.Instance.PlaySFX("boss_death");
        OnDeath?.Invoke();
    }
}