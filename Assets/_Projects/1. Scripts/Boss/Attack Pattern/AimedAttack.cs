using UnityEngine;
using System.Collections;

public class AimedAttack : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform player;
    [SerializeField] private float damage;

    [Header("ATTACK")]
    [SerializeField] private int shotCount = 3;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float timeBetweenShots = 0.2f;
    [SerializeField] private float telegraphTime = 0.35f;

    private float speedMultiplier = 1;

    public IEnumerator Fire(){
        yield return new WaitForSeconds(telegraphTime);

        for(int i = 0; i < shotCount; i++){
            if(player == null) yield break;

            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            Spawn(dir);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    void Spawn(Vector2 dir){
        if(projectilePool == null) return;
        Projectile p = projectilePool.Get();
        if(p == null) return;
        p.transform.position = transform.position;
        p.gameObject.SetActive(true);
        p.Launch(dir, Projectile.Owner.Enemy, damage, projectileSpeed * speedMultiplier);
    }

    public void SetSpeedMultiplier(float value) => speedMultiplier = value;
}