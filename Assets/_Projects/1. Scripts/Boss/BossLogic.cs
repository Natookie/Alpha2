using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class BossLogic : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private BossStats bossStats;
    [SerializeField] private BossMovement bossMovement;
    [SerializeField] private Transform player;

    [Header("FIGHT")]
    [SerializeField] private float patternInterval = 2.2f;

    [Header("PHASES")]
    [SerializeField] private float rocketPhaseHealthPercent = 0.25f;
    [SerializeField, ReadOnly] private int currentPhase = 1;

    [Header("ROCKET PHASE")]
    [SerializeField] private RocketAttack rocketAttack;
    [SerializeField] private float rocketInterval = 4f;

    [Header("CAMERA INTRO")]
    [SerializeField] private float rocketPhaseCameraTime = 1.2f;

    [Header("ATTACK PATTERNS")]
    [SerializeField] private RadialBurstAttack radialAttack;
    [SerializeField] private SpiralAttack spiralAttack;
    [SerializeField] private AimedAttack aimedAttack;
    [SerializeField] private ShotgunAttack shotgunAttack;

    private bool isFighting;
    private bool rocketPhaseActive;
    private bool rocketPhaseTransitioning;
    private float nextPatternTime;
    private float nextRocketTime;

    void Awake(){
        if(bossStats == null) bossStats = GetComponent<BossStats>();
        if(bossMovement == null) bossMovement = GetComponent<BossMovement>();
    }

    void OnEnable(){
        if(bossStats != null){
            bossStats.OnDeath += HandleDeath;
            bossStats.OnHealthChanged += HandleHealthChanged;
        }
    }

    void OnDisable(){
        if(bossStats != null){
            bossStats.OnDeath -= HandleDeath;
            bossStats.OnHealthChanged -= HandleHealthChanged;
            bossStats.SetInvulnerable(false);
        }
    }

    public void BeginFight(){
        if(isFighting) return;
        isFighting = true;
        nextPatternTime = Time.time + patternInterval;
        if(bossMovement != null) bossMovement.BeginMovement();
    }

    void Update(){
        if(!isFighting) return;
        if(bossStats.IsDead) return;
        if(rocketPhaseTransitioning) return;

        if(Time.time >= nextPatternTime){
            nextPatternTime = Time.time + patternInterval;
            StartCoroutine(RunPattern());
        }

        if(rocketPhaseActive && Time.time >= nextRocketTime){
            nextRocketTime = Time.time + rocketInterval;
            if(rocketAttack != null) StartCoroutine(rocketAttack.Fire());
        }
    }

    IEnumerator RunPattern(){
        if(rocketPhaseActive){
            int pick = Random.Range(0, 4);
            switch(pick){
                case 0: yield return StartCoroutine(radialAttack.Fire()); break;
                case 1: yield return StartCoroutine(spiralAttack.Fire()); break;
                case 2: yield return StartCoroutine(aimedAttack.Fire()); break;
                case 3: yield return StartCoroutine(shotgunAttack.Fire()); break;
            }
        }else if(currentPhase == 1){
            int pick = Random.Range(0, 3);
            switch(pick){
                case 0: yield return StartCoroutine(radialAttack.Fire()); break;
                case 1: yield return StartCoroutine(spiralAttack.Fire()); break;
                case 2: yield return StartCoroutine(shotgunAttack.Fire()); break;
            }
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(aimedAttack.Fire());
        }else{
            int pick = Random.Range(0, 4);
            switch(pick){
                case 0: yield return StartCoroutine(radialAttack.Fire()); break;
                case 1: yield return StartCoroutine(spiralAttack.Fire()); break;
                case 2: yield return StartCoroutine(aimedAttack.Fire()); break;
                case 3: yield return StartCoroutine(shotgunAttack.Fire()); break;
            }
        }
    }

    void HandleHealthChanged(float health, float maxHealth){
        float ratio = health / maxHealth;

        if(currentPhase == 1 && ratio <= 0.5f){
            currentPhase = 2;
            patternInterval *= 0.7f;
        }

        if(!rocketPhaseActive && ratio <= rocketPhaseHealthPercent){
            rocketPhaseActive = true;
            StartCoroutine(EnterRocketPhase());
        }
    }

    IEnumerator EnterRocketPhase(){
        rocketPhaseTransitioning = true;
        if(bossStats != null) bossStats.SetInvulnerable(true);

        if(bossMovement != null) bossMovement.StopMovement();

        foreach(var atk in new MonoBehaviour[]{ radialAttack, spiralAttack, aimedAttack, shotgunAttack }){
            if(atk != null) atk.StopAllCoroutines();
        }

        if(CameraController.Instance != null){
            CameraController.Instance.SetCinemachineTarget(transform);
            CameraController.Instance.Shake(0.5f, 0.8f);
            CameraController.Instance.HitStop(0.15f, 0f);
        }

        AudioManager.Instance.PlaySFX("boss_phase2");
        yield return new WaitForSeconds(rocketPhaseCameraTime);

        radialAttack.SetSpeedMultiplier(2f);
        spiralAttack.SetSpeedMultiplier(2f);
        aimedAttack.SetSpeedMultiplier(2f);
        shotgunAttack.SetSpeedMultiplier(2f);
        bossStats.defense = 5f;

        if(CameraController.Instance != null && player != null) CameraController.Instance.SetCinemachineTarget(player);
        if(bossStats != null) bossStats.SetInvulnerable(false);

        nextPatternTime = Time.time + patternInterval;
        nextRocketTime = Time.time + 0.5f;
        rocketPhaseTransitioning = false;

        if(rocketAttack != null) StartCoroutine(rocketAttack.Fire());
    }

    void HandleDeath(){
        isFighting = false;
        rocketPhaseActive = false;
        rocketPhaseTransitioning = false;
        StopAllCoroutines();
        if(bossMovement != null) bossMovement.StopMovement();
        if(bossStats != null) bossStats.SetInvulnerable(false);
    }
}