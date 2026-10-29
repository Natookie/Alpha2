using UnityEngine;
using NaughtyAttributes;
using System;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("STATS")]
    [ReadOnly] public float health;
    [SerializeField] private float maxHealth;
    public float attackDamage;

    [Header("REFERENCES")]
    [SerializeField] private PlayerLogic playerLogic;
    [SerializeField] private PlayerUI playerUI;

    public event Action OnDeath;
    public bool IsDead => health <= 0f;

    void Start(){
        health  = maxHealth;
        if(playerLogic == null) playerLogic = GetComponent<PlayerLogic>();
        if(playerUI == null) playerUI = GetComponent<PlayerUI>();
    }

    public bool TakeDamage(float amount){
        if(IsDead) return false;
        if(playerLogic != null && playerLogic.IsInvulnerable) return false;

        health -= amount;
        if(health < 0f) health = 0f;

        AudioManager.Instance?.PlaySFX("player_take_damage");
        if(playerUI != null) playerUI.UpdateHealth((int)health);
        CameraController.Instance?.HitStop();

        if(health <= 0f) OnDeath?.Invoke();
        return true;
    }
}