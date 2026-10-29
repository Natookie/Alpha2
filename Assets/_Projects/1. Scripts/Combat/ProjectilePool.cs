using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    [Header("POOL")]
    [SerializeField] private Projectile prefab;
    [SerializeField] private int initialSize = 16;

    private readonly List<Projectile> pool = new List<Projectile>();

    void Awake(){
        for(int i = 0; i < initialSize; i++){
            Projectile p = Instantiate(prefab, transform);
            p.gameObject.SetActive(false);
            pool.Add(p);
        }
    }

    public Projectile Get(){
        for(int i = 0; i < pool.Count; i++){
            if(!pool[i].gameObject.activeInHierarchy){
                return pool[i];
            }
        }

        Projectile extra = Instantiate(prefab, transform);
        extra.gameObject.SetActive(false);
        pool.Add(extra);
        return extra;
    }
}