using UnityEngine;
using System.Collections;

public class HitFlash : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private SpriteRenderer[] renderers;
    [Space(10)]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.06f;

    private MaterialPropertyBlock block;
    private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");
    private static readonly int FlashColorId  = Shader.PropertyToID("_FlashColor");

    private Coroutine flashCoroutine;

    void Awake(){
        if(renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<SpriteRenderer>();

        block = new MaterialPropertyBlock();
    }

    public void Flash(){
        Flash(flashDuration);
    }

    public void Flash(float duration){
        if(flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine(duration));
    }

    IEnumerator FlashRoutine(float duration){
        SetFlash(1f);

        yield return new WaitForSecondsRealtime(duration);

        SetFlash(0f);
        flashCoroutine = null;
    }

    void SetFlash(float amount){
        foreach(var r in renderers){
            if(r == null) continue;
            r.GetPropertyBlock(block);
            block.SetFloat(FlashAmountId, amount);
            block.SetColor(FlashColorId, flashColor);
            r.SetPropertyBlock(block);
        }
    }
}