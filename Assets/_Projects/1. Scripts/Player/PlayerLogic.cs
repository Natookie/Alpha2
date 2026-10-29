using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using System.Collections;

public class PlayerLogic : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] private float playerSpeed;
    [Space(10)]
    [SerializeField] private float playerDashDistance;
    [SerializeField] private float playerDashCooldown;
    private const float DASH_IFRAME_DURATION = .3f;

    [Header("FOOTSTEP")]
    [SerializeField] private float stepInterval = 0.3f;

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb;
    [Space(10)]
    [SerializeField, ReadOnly] private Vector2 movementInput;
    [SerializeField, ReadOnly] private Vector2 facingDirection = Vector2.down;

    private bool isDashing;
    private bool isInvulnerable;
    private float lastDashTime = -Mathf.Infinity;
    private float stepTimer;

    public Vector2 MovementInput => movementInput;
    public Vector2 FacingDirection => facingDirection;
    public bool IsInvulnerable => isInvulnerable;
    public bool IsDashing => isDashing;
    public bool IsMoving => movementInput != Vector2.zero;

    void Update(){
        HandleMovementInput();
        HandleDashInput();
    }

    void HandleMovementInput(){
        if(isDashing) return;

        movementInput = Vector2.zero;

        if(Keyboard.current.wKey.isPressed) movementInput += Vector2.up;
        if(Keyboard.current.sKey.isPressed) movementInput += Vector2.down;
        if(Keyboard.current.aKey.isPressed) movementInput += Vector2.left;
        if(Keyboard.current.dKey.isPressed) movementInput += Vector2.right;

        movementInput = movementInput.normalized;

        UpdateFacingDirection();
    }

    void UpdateFacingDirection(){
        int x = 0;
        int y = 0;

        if(Keyboard.current.aKey.isPressed) x -= 1;
        if(Keyboard.current.dKey.isPressed) x += 1;
        if(Keyboard.current.sKey.isPressed) y -= 1;
        if(Keyboard.current.wKey.isPressed) y += 1;

        if(x == 0 && y == 0) return;

        if(x != 0 && y != 0) facingDirection = PickNewestAxis(x, y);
        else facingDirection = new Vector2(x, y);
    }

    private int lastHorizontalFrame = -1;
    private int lastVerticalFrame = -1;
    private int prevX = 0;
    private int prevY = 0;

    Vector2 PickNewestAxis(int x, int y){
        if(x != prevX) lastHorizontalFrame = Time.frameCount;
        if(y != prevY) lastVerticalFrame = Time.frameCount;
        prevX = x;
        prevY = y;

        if(lastHorizontalFrame > lastVerticalFrame) return new Vector2(x, 0);
        return new Vector2(0, y);
    }

    void HandleDashInput(){
        if(isDashing) return;
        if(Time.time < lastDashTime + playerDashCooldown) return;

        if(Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame){
            Vector2 dir = movementInput != Vector2.zero ? movementInput : facingDirection;
            StartCoroutine(Dash(dir));
        }
    }

    void FixedUpdate(){
        if(isDashing) return;

        rb.MovePosition(rb.position + movementInput * playerSpeed * Time.fixedDeltaTime);

        HandleFootstep();
    }

    void HandleFootstep(){
        if(!IsMoving){
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.fixedDeltaTime;
        if(stepTimer <= 0f){
            AudioManager.Instance?.PlaySFX("player_step");
            stepTimer = stepInterval;
        }
    }

    IEnumerator Dash(Vector2 direction){
        isDashing = true;
        isInvulnerable = true;
        lastDashTime = Time.time;

        facingDirection = direction.normalized;
        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + direction.normalized * playerDashDistance;

        float dashDuration = 0.15f;
        float elapsed = 0f;

        while(elapsed < dashDuration){
            float t = elapsed / dashDuration;
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, t));
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
        isDashing = false;

        AudioManager.Instance?.PlaySFX("player_dash");

        float iframeRemaining = DASH_IFRAME_DURATION - dashDuration;
        if(iframeRemaining > 0f) yield return new WaitForSeconds(iframeRemaining);

        isInvulnerable = false;
    }
}