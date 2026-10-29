using UnityEngine;
using Nova;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private UIBlock2D[] heart;
    [SerializeField] private Color activeHealth;
    [SerializeField] private Color inactiveHealth;

    private int currentIndex;
    private int heartLength => heart.Length;

    void Start() => currentIndex = heartLength;

    public void UpdateHealth(int value){
        bool isPositive = currentIndex > value;
        currentIndex += isPositive ? 1 : -1;
        
        for(int i = 0; i < heartLength; i++){
            if(i < currentIndex) heart[i].Color = activeHealth;
            else heart[i].Color = inactiveHealth;
        }
    } 
}