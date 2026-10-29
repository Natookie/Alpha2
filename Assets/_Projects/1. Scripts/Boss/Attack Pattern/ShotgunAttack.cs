using UnityEngine;
using System.Collections;

public class ShotgunAttack : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform player;
    [SerializeField] private float damage;

    [Header("ATTACK")]
    [SerializeField] private int projectileCount = 5;
    [SerializeField] private float spreadAngle = 30f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float telegraphTime = 0.35f;

    private float speedMultiplier = 1;

    public IEnumerator Fire(){
        yield return new WaitForSeconds(telegraphTime);

        if(player == null) yield break;

        Vector2 baseDir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        for(int i = 0; i < projectileCount; i++){
            float offset = (i - (projectileCount - 1) * 0.5f) * (spreadAngle / (projectileCount - 1));
            float angle = (baseAngle + offset) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Spawn(dir);
        }

        yield return new WaitForSeconds(0.2f);
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