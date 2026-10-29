using UnityEngine;

public class PlayerDust : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private ParticleSystem walkDust;
    [SerializeField] private ParticleSystem dashDust;
    [Space(10)]
    [SerializeField] private PlayerLogic playerLogic;

    [Header("WALK")]
    [SerializeField] private float walkDustInterval = 0.25f;

    private float walkDustTimer;

    void Awake(){
        if(playerLogic == null) playerLogic = GetComponent<PlayerLogic>();
    }

    void Update() => HandleWalkDust();

    void HandleWalkDust(){
        if(playerLogic.IsDashing) return;

        if(playerLogic.IsMoving){
            walkDustTimer -= Time.deltaTime;
            if(walkDustTimer <= 0f){
                EmitWalkDust();
                walkDustTimer = walkDustInterval;
            }
        }else walkDustTimer = 0f;
    }

    void EmitWalkDust(){
        if(walkDust == null) return;

        walkDust.transform.position = playerLogic.transform.position;
        walkDust.Play();
    }

    public void PlayDashDust(){
        if(dashDust == null) return;

        Vector2 facing = playerLogic.FacingDirection;
        float facingAngle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;

        dashDust.transform.rotation = Quaternion.Euler(0, 0, facingAngle + 90f);
        dashDust.transform.position = playerLogic.transform.position;
        dashDust.Play();
    }
}