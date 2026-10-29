using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Animator animator;
    [Space(10)]
    [SerializeField] private PlayerLogic playerLogic;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerStats playerStats;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int DirXHash  = Animator.StringToHash("DirX");
    private static readonly int DirYHash  = Animator.StringToHash("DirY");
    private static readonly int IsDeathHash = Animator.StringToHash("IsDeath");

    private enum PlayerState { Idle = 0, Move = 1, Dash = 2, Attack = 3 }

    void Awake(){
        if(playerLogic == null) playerLogic = GetComponent<PlayerLogic>();
        if(playerCombat == null) playerCombat = GetComponent<PlayerCombat>();
        if(playerStats == null) playerStats = GetComponent<PlayerStats>();
        if(animator == null) animator = GetComponentInChildren<Animator>();
    }

    void LateUpdate(){
        if(animator == null) return;

        if(playerStats != null && playerStats.IsDead){
            animator.SetBool(IsDeathHash, true);
            return;
        }

        animator.SetInteger(StateHash, (int)GetCurrentState());

        Vector2 dir = playerLogic.FacingDirection;
        animator.SetFloat(DirXHash, Mathf.RoundToInt(dir.x));
        animator.SetFloat(DirYHash, Mathf.RoundToInt(dir.y));
    }

    PlayerState GetCurrentState(){
        if(playerCombat != null && playerCombat.IsAttacking) return PlayerState.Attack;
        if(playerLogic.IsDashing) return PlayerState.Dash;
        if(playerLogic.IsMoving) return PlayerState.Move;
        return PlayerState.Idle;
    }
}