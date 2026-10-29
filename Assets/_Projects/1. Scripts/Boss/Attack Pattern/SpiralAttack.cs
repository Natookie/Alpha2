using UnityEngine;
using System.Collections;

public class SpiralAttack : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private float damage;

    [Header("ATTACK")]
    [SerializeField] private int shotCount = 20;
    [SerializeField] private float angleStep = 18f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float timeBetweenShots = 0.06f;
    [SerializeField] private float telegraphTime = 0.3f;

    private float speedMultiplier = 1;

    public IEnumerator Fire(){
        yield return new WaitForSeconds(telegraphTime);

        float angle = 0f;
        for(int i = 0; i < shotCount; i++){
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            Spawn(dir);
            angle += angleStep;
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