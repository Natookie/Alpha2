using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitFirePoint : MonoBehaviour
{
    [Header("ORBIT")]
    [SerializeField] private Transform player;
    [SerializeField] private float radius = 0.6f;

    void LateUpdate(){
        if(player == null) return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 toMouse = mouseWorld - (Vector2)player.position;

        if(toMouse.sqrMagnitude < 0.0001f) return;

        Vector2 dir = toMouse.normalized;
        transform.position = (Vector2)player.position + dir * radius;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}