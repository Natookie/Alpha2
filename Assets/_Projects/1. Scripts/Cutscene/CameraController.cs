using UnityEngine;
using Unity.Cinemachine;
using NaughtyAttributes;
using System.Collections;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("ORTHOGRAPHIC")]
    [SerializeField] private float defaultOrthoSize = 5f;
    [SerializeField] private float minOrthoSize = 2f;
    [SerializeField] private float maxOrthoSize = 15f;
    [Space(10)]
    [SerializeField] private bool smoothZoom = true;
    [SerializeField] private float zoomLerpSpeed = 5f;
    [SerializeField, ReadOnly] private float targetOrthoSize;

    [Header("SHAKE")]
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] private float defaultShakeDuration = 0.25f;
    [SerializeField] private float defaultShakeMagnitude = 0.5f;
    [Space(10)]
    [SerializeField] private AnimationCurve shakeFalloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("HIT STOP")]
    [SerializeField] private float defaultHitStopDuration = 0.05f;
    [SerializeField] private float defaultHitStopScale = 0f;

    [Header("REFERENCES")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private CanvasGroup overlay;

    private float shakeTimer;
    private float shakeDuration;
    private float shakeMagnitude;
    private bool wasConfinerEnabled;
    private Coroutine hitStopCoroutine;
    private Coroutine flashCoroutine;

    public float CurrentOrthoSize => cinemachineCamera.Lens.OrthographicSize;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }

        if(cinemachineCamera == null) cinemachineCamera = GetComponent<CinemachineCamera>();
        if(confiner == null) confiner = GetComponent<CinemachineConfiner2D>();
        if(noise == null) noise = GetComponent<CinemachineBasicMultiChannelPerlin>();

        if(confiner != null) wasConfinerEnabled = confiner.enabled;

        if(noise != null){
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 1f;
        }

        SetOrthoSizeInstant(defaultOrthoSize);
    }

    void Update(){
        HandleConfinerState();
        HandleZoom();
        HandleShake();
    }

    #region ORTHO & CONFINER
    void HandleConfinerState(){
        if(confiner == null) return;

        if(confiner.enabled && !wasConfinerEnabled) StartCoroutine(InvalidateDelay());
        wasConfinerEnabled = confiner.enabled;
    }
    IEnumerator InvalidateDelay(){
        yield return new WaitForEndOfFrame();
        confiner.InvalidateLensCache();
    }

    public void SetOrthoSize(float size){
        targetOrthoSize = Mathf.Clamp(size, minOrthoSize, maxOrthoSize);
        StartCoroutine(InvalidateDelay());
        if(!smoothZoom) SetOrthoSizeInstant(targetOrthoSize);
    }

    public void SetOrthoSizeInstant(float size){
        targetOrthoSize = Mathf.Clamp(size, minOrthoSize, maxOrthoSize);

        var lens = cinemachineCamera.Lens;
        lens.OrthographicSize = targetOrthoSize;
        cinemachineCamera.Lens = lens;

        InvalidateConfinerIfNeeded();
    }
    
    void InvalidateConfinerIfNeeded(){
        if(confiner != null && confiner.enabled) confiner.InvalidateLensCache();
    }

    public void ResetOrthoSize() => SetOrthoSize(defaultOrthoSize);
    #endregion


    #region ZOOM & TARGET
    void HandleZoom(){
        if(!smoothZoom) return;

        float current = cinemachineCamera.Lens.OrthographicSize;
        if(Mathf.Approximately(current, targetOrthoSize)) return;

        float next = Mathf.Lerp(current, targetOrthoSize, zoomLerpSpeed * Time.deltaTime);

        if(Mathf.Abs(next - targetOrthoSize) < 0.01f) next = targetOrthoSize;

        var lens = cinemachineCamera.Lens;
        lens.OrthographicSize = next;
        cinemachineCamera.Lens = lens;

        InvalidateConfinerIfNeeded();
    }

    public void SetCinemachineTarget(Transform target){
        if(cinemachineCamera == null || target == null) return;
        cinemachineCamera.Follow = target;
    }
    #endregion

    #region SHAKE
    public void Shake() => Shake(defaultShakeDuration, defaultShakeMagnitude);
    public void Shake(float duration, float magnitude){
        if(noise == null) return;

        if(shakeTimer > 0f && magnitude < shakeMagnitude) return;

        shakeDuration = Mathf.Max(0.01f, duration);
        shakeMagnitude = magnitude;
        shakeTimer = shakeDuration;
    }
    void HandleShake(){
        if(noise == null) return;

        if(shakeTimer <= 0f) return;

        shakeTimer -= Time.deltaTime;

        if(shakeTimer <= 0f){
            noise.AmplitudeGain = 0f;
            return;
        }

        float t = 1f - (shakeTimer / shakeDuration);
        float falloff = shakeFalloff.Evaluate(t);
        noise.AmplitudeGain = shakeMagnitude * falloff;
    }
    #endregion

    #region HIT FRAME
    public void HitStop() => HitStop(defaultHitStopDuration, defaultHitStopScale);
    public void HitStop(float duration, float timeScale){
        if(hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        Flash();
        hitStopCoroutine = StartCoroutine(HitStopRoutine(duration, timeScale));
    }
    IEnumerator HitStopRoutine(float duration, float timeScale){
        float originalScale = Time.timeScale;
        float originalFixedDelta = Time.fixedDeltaTime;

        Time.timeScale = Mathf.Clamp01(timeScale);
        Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalScale;
        Time.fixedDeltaTime = originalFixedDelta;

        hitStopCoroutine = null;
    }

    public void Flash() => Flash(defaultHitStopDuration/3f);
    public void Flash(float duration){
        if(flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine(duration));
    }
    IEnumerator FlashRoutine(float duration){
        float half = duration * 0.5f;

        float t = 0f;
        while(t < half){
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.Lerp(0f, 1f, Mathf.Clamp01(t / half));
            yield return null;
        }

        t = 0f;
        while(t < half){
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(t / half));
            yield return null;
        }

        overlay.alpha = 0f;
        flashCoroutine = null;
    }
    #endregion
}