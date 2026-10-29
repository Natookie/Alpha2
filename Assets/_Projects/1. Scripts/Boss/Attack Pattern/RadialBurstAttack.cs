using UnityEngine;
using System.Collections;

public class RadialBurstAttack : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private float damage;

    [Header("ATTACK")]
    [SerializeField] private int projectileCount = 12;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float telegraphTime = 0.4f;

    private float speedMultiplier = 1;

    public IEnumerator Fire(){
        yield return new WaitForSeconds(telegraphTime);

        for(int i = 0; i < projectileCount; i++){
            float angle = i * (360f / projectileCount);
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
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