using UnityEngine;
using System.Collections;
using Nova;

public class BossUI : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D healthFill;
    [SerializeField] private UIBlock2D healthDelayFill;

    [Header("ANIMATION")]
    [SerializeField] private float delayHoldTime = 0.25f;
    [SerializeField] private float fillLerpDuration = 0.4f;

    public void UpdateHealth(float health, float maxHealth){
        float targetFill = Mathf.Clamp01(health / maxHealth);

        float currentFill = healthFill.Size.X.Percent;
        if(Mathf.Approximately(targetFill, currentFill)) return;

        StopAllCoroutines();
        StartCoroutine(HealthSequence(currentFill, targetFill));
    }

    IEnumerator HealthSequence(float startFill, float targetFill){
        bool isDamage = targetFill < startFill;

        if(isDamage){
            float lostAbs = startFill - targetFill;
            healthDelayFill.Size.X.Percent = lostAbs / startFill;

            yield return new WaitForSeconds(delayHoldTime);

            float t = 0f;
            while(t < 1f){
                t += Time.deltaTime / fillLerpDuration;
                float e = Mathf.SmoothStep(0f, 1f, t);

                float newHealth = Mathf.Lerp(startFill, targetFill, e);
                float newLostAbs = Mathf.Lerp(lostAbs, 0f, e);

                healthFill.Size.X.Percent = newHealth;
                healthDelayFill.Size.X.Percent = newHealth > 0.0001f
                    ? newLostAbs / newHealth
                    : 0f;

                yield return null;
            }
        }else{
            healthDelayFill.Size.X.Percent = 0f;

            float t = 0f;
            while(t < 1f){
                t += Time.deltaTime / fillLerpDuration;
                float e = Mathf.SmoothStep(0f, 1f, t);

                healthFill.Size.X.Percent = Mathf.Lerp(startFill, targetFill, e);
                yield return null;
            }
        }

        healthFill.Size.X.Percent = targetFill;
        healthDelayFill.Size.X.Percent = 0f;
    }
}