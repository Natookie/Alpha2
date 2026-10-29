using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("ATTACK")]
    [SerializeField] private int projectileCount = 4;
    [SerializeField] private float timeBetweenShots = 0.08f;
    [Space(10)]
    [SerializeField, ReadOnly] private bool isAttacking;
    [SerializeField, ReadOnly] private Vector2 facingDirection = Vector2.down;

    [Header("REFERENCES")]
    [SerializeField] private PlayerLogic playerLogic;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Animator animator;

    public bool IsAttacking => isAttacking;
    public Vector2 FacingDirection => facingDirection;

    void Awake(){
        if(playerLogic == null) playerLogic = GetComponent<PlayerLogic>();
    }

    void Update(){
        UpdateFacingDirection();
        HandleAttackInput();
    }

    void UpdateFacingDirection(){
        if(Mouse.current == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 toMouse = mouseWorld - (Vector2)transform.position;

        if(toMouse.sqrMagnitude < 0.0001f) return;

        facingDirection = SnapToCardinal(toMouse);
    }

    Vector2 SnapToCardinal(Vector2 dir){
        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) return dir.x > 0f ? Vector2.right : Vector2.left;
        return dir.y > 0f ? Vector2.up : Vector2.down;
    }

    float GetAttackClipLength(){
        var controller = animator.runtimeAnimatorController;
        if(controller == null) return 0.3f;

        foreach(var clip in controller.animationClips){
            if(clip.name.Contains("Attack")) return clip.length;
        }
        return 0.3f;
    }

    void HandleAttackInput(){
        if(isAttacking) return;
        if(playerLogic.IsDashing) return;

        if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            StartCoroutine(Attack());
    }

    IEnumerator Attack(){
        isAttacking = true;

        float fireWindow = (projectileCount - 1) * timeBetweenShots;
        float clipLength = GetAttackClipLength();
        float fireStart = Mathf.Max(0f, (clipLength - fireWindow) * 0.5f);

        if(fireStart > 0f) yield return new WaitForSeconds(fireStart);

        for(int i = 0; i < projectileCount; i++){
            FireProjectile();
            if(i < projectileCount - 1) yield return new WaitForSeconds(timeBetweenShots);
        }

        float elapsed = fireStart + fireWindow;
        float remaining = clipLength - elapsed;
        if(remaining > 0f) yield return new WaitForSeconds(remaining);

        isAttacking = false;
    }

    void FireProjectile(){
        if(projectilePool == null) return;
        if(firePoint == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 toMouse = mouseWorld - (Vector2)firePoint.position;

        if(toMouse.sqrMagnitude < 0.0001f) return;

        Projectile p = projectilePool.Get();
        if(p == null) return;

        float damage = playerStats != null ? playerStats.attackDamage : 1f;
        p.transform.position = firePoint.position;
        p.transform.rotation = firePoint.rotation;
        p.gameObject.SetActive(true);
        p.Launch(toMouse.normalized, Projectile.Owner.Player, damage, 25f);
        AudioManager.Instance.PlaySFX("player_attack");
    }
}