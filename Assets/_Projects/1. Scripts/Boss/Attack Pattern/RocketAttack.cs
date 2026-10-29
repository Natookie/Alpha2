using UnityEngine;
using System.Collections;

public class RocketAttack : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private GameObject warningCirclePrefab;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float damage;

    [Header("ROCKET")]
    [SerializeField] private float warningDuration = 2.5f;
    [SerializeField] private float warningStartScale = 1.5f;
    [SerializeField] private float warningEndScale = 1f;
    [SerializeField] private float rocketSpawnHeight = 20f;
    [SerializeField] private float rocketFallSpeed = 30f;
    [SerializeField] private float rocketDamage = 25f;
    [SerializeField] private float rocketRadius = 1.5f;

    public IEnumerator Fire(){
        if(player == null) yield break;

        GameObject warning = null;
        if(warningCirclePrefab != null){
            warning = Instantiate(warningCirclePrefab);
            warning.transform.position = player.position;
            warning.transform.localScale = Vector3.one * warningStartScale;
        }

        float elapsed = 0f;
        while(elapsed < warningDuration){
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / warningDuration);
            float scale = Mathf.Lerp(warningStartScale, warningEndScale, t);

            if(warning != null){
                warning.transform.position = player.position;
                warning.transform.localScale = Vector3.one * scale;
            }
            yield return null;
        }

        Vector2 targetPos = player.position;

        if(warning != null) Destroy(warning);

        if(rocketPrefab != null){
            GameObject rocket = Instantiate(rocketPrefab);
            rocket.transform.position = new Vector3(targetPos.x, targetPos.y + rocketSpawnHeight, 0f);

            RocketProjectile rp = rocket.GetComponent<RocketProjectile>();
            if(rp != null){
                rp.Launch(targetPos, rocketFallSpeed, rocketDamage, rocketRadius);
            }
        }
    }
}