using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class BossMovement : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float arriveThreshold = 0.05f;

    [Header("REPOSITIONING")]
    [SerializeField] private float minRepositionDistance = 2f;
    [SerializeField] private float maxRepositionDistance = 5f;
    [SerializeField] private float minIdleTime = 0.5f;
    [SerializeField] private float maxIdleTime = 1.5f;

    [Header("BOUNDS")]
    [SerializeField] private Collider2D arenaBounds;

    [Header("REFERENCES")]
    [SerializeField] private Transform player;

    private Vector2 homePosition;
    private Vector2 targetPosition;
    private bool isMoving;

    public bool IsMoving => isMoving;

    void Start(){
        homePosition = transform.position;
        targetPosition = homePosition;
    }

    public void BeginMovement(){
        StartCoroutine(MovementLoop());
    }

    public void StopMovement(){
        StopAllCoroutines();
        isMoving = false;
    }

    IEnumerator MovementLoop(){
        while(true){
            Vector2 direction = Random.insideUnitCircle.normalized;
            if(direction == Vector2.zero) direction = Vector2.right;

            float distance = Random.Range(minRepositionDistance, maxRepositionDistance);
            Vector2 candidate = (Vector2)player.position + direction * distance;
            candidate = ClampToArena(candidate);

            targetPosition = candidate;
            isMoving = true;

            while(Vector2.Distance(transform.position, targetPosition) > arriveThreshold){
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            isMoving = false;

            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
        }
    }

    Vector2 ClampToArena(Vector2 point){
        if(arenaBounds == null) return point;

        Bounds b = arenaBounds.bounds;
        point.x = Mathf.Clamp(point.x, b.min.x + 0.5f, b.max.x - 0.5f);
        point.y = Mathf.Clamp(point.y, b.min.y + 0.5f, b.max.y - 0.5f);
        return point;
    }
}