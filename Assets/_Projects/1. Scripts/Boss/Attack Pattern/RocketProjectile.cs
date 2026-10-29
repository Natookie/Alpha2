using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RocketProjectile : MonoBehaviour
{
    [Header("EXPLOSION FX")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float lifetimeAfterLanding = 0.5f;

    private Vector2 targetPos;
    private float fallSpeed;
    private float damage;
    private float radius;
    private bool hasLanded;
    private Collider2D rocketCollider;
    private SpriteRenderer sr;

    void Awake(){
        rocketCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Launch(Vector2 target, float speed, float dmg, float aoeRadius){
        targetPos = target;
        fallSpeed = speed;
        damage = dmg;
        radius = aoeRadius;
        hasLanded = false;

        if(rocketCollider != null) rocketCollider.enabled = false;
    }

    void Update(){
        if(hasLanded) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            fallSpeed * Time.deltaTime
        );

        if(Vector2.Distance(transform.position, targetPos) <= 0.05f)
            Land();
    }

    void Land(){
        hasLanded = true;

        if(explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach(var hit in hits){
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if(target == null) continue;
            if(hit.GetComponentInParent<BossStats>() != null) continue;
            target.TakeDamage(damage);
        }

        AudioManager.Instance.PlaySFX("rocket_explosion");
        sr.enabled = false;
        Destroy(gameObject, lifetimeAfterLanding);
    }
}