using UnityEngine;

public static class Vector3IntExtensions
{
    public static Vector2Int ToVector2Int(this Vector3Int v){
        return new Vector2Int(v.x, v.y);
    }
}