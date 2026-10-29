using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public enum Owner { Player, Enemy }

    [Header("STATS")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;

    [Header("VISUALS")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color playerColor;
    [SerializeField] private Color enemyColor;

    private Vector2 direction;
    private float lifeTimer;
    private bool isActive;
    private float damage;
    private float speedValue;
    private Owner currentOwner;

    public float Damage => damage;

    void OnEnable(){
        lifeTimer = lifetime;
        isActive = true;
    }

    void Update(){
        if(!isActive) return;

        transform.position += (Vector3)(direction * speedValue * Time.deltaTime);

        lifeTimer -= Time.deltaTime;
        if(lifeTimer <= 0f) Deactivate();
    }

    public void Launch(Vector2 dir, Owner owner, float damageAmount, float speedOverride = -1f){
        currentOwner = owner;
        damage = damageAmount;
        speedValue = speedOverride >= 0f ? speedOverride : speed;
        ApplyVisuals(owner);

        direction = dir.normalized;
        lifeTimer = lifetime;
        isActive = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Launch(Vector2 dir, Owner owner){
        Launch(dir, owner, 1f, -1f);
    }

    void ApplyVisuals(Owner owner){
        if(spriteRenderer == null) return;
        spriteRenderer.color = owner == Owner.Player ? playerColor : enemyColor;
    }

    void OnTriggerEnter2D(Collider2D other){
        if(!isActive) return;
        if(IsSameTeam(other)) return;

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if(target == null) return;

        bool landed = target.TakeDamage(damage);
        if(landed) Deactivate();
    }

    bool IsSameTeam(Collider2D other){
        PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();
        BossStats bossStats = other.GetComponentInParent<BossStats>();

        if(currentOwner == Owner.Player && playerStats != null) return true;
        if(currentOwner == Owner.Enemy && bossStats != null) return true;

        return false;
    }

    void Deactivate(){
        if(!isActive) return;
        isActive = false;
        StartCoroutine(DeactivateAfterDelay());
    }

    IEnumerator DeactivateAfterDelay(){
        yield return new WaitForSeconds(.08f);
        gameObject.SetActive(false);
    }
}