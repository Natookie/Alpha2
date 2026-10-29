using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using NaughtyAttributes;
using Unity.Cinemachine;

public class CutsceneTrigger : MonoBehaviour
{
    public enum TriggerMode { Auto, OnTriggerEnter, Manual }

    [Header("CUTSCENE")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TriggerMode triggerMode = TriggerMode.OnTriggerEnter;
    [Space(10)]
    [SerializeField] private bool playOnce = true;
    [SerializeField] private bool lockPlayerDuringCutscene = true;
    [SerializeField] private bool setCinemachineTarget = true;
    [ShowIf("setCinemachineTarget"), SerializeField] private Transform ccTarget;

    [Header("OBJECT REF")]
    [SerializeField] private GameObject[] obj;

    [Header("REFERENCES")]
    [SerializeField] private PlayerLogic playerLogic;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerDust playerDust;
    [SerializeField] private CinemachineCamera cc;

    [Header("EVENTS")]
    [SerializeField] private UnityEvent onCutsceneStarted;
    [SerializeField] private UnityEvent onCutsceneEnded;

    [Header("DEBUG")]
    [SerializeField, ReadOnly] private bool hasPlayed;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    void Start(){
        if(director == null) director = GetComponent<PlayableDirector>();
        if(playerLogic == null) playerLogic = FindFirstObjectByType<PlayerLogic>();
        if(playerCombat == null) playerCombat = FindFirstObjectByType<PlayerCombat>();
        if(playerDust == null) playerDust = FindFirstObjectByType<PlayerDust>();

        if(director != null) director.stopped += OnCutsceneStopped;
        if(triggerMode == TriggerMode.Auto && CanPlay()) Play();

        SetAllObjActive(false);
    }

    void OnDestroy(){
        if(director != null) director.stopped -= OnCutsceneStopped;
    }

    void OnTriggerEnter2D(Collider2D other){
        if(triggerMode != TriggerMode.OnTriggerEnter) return;
        if(!other.CompareTag("Player")) return;
        if(!CanPlay()) return;

        SetAllObjActive(true);
        Play();
    }

    bool CanPlay(){
        if(isPlaying) return false;
        if(playOnce && hasPlayed) return false;
        if(director == null) return false;
        return true;
    }

    public void Play(){
        if(!CanPlay()) return;

        hasPlayed = true;
        isPlaying = true;

        if(lockPlayerDuringCutscene) SetPlayerControl(false);
        director.Play();

        onCutsceneStarted?.Invoke();
    }

    void OnCutsceneStopped(PlayableDirector _){
        isPlaying = false;

        if(lockPlayerDuringCutscene) SetPlayerControl(true);
        if(cc != null && ccTarget != null) cc.Follow = ccTarget;

        onCutsceneEnded?.Invoke();
    }

    void SetPlayerControl(bool enabled){
        if(playerLogic != null) playerLogic.enabled = enabled;
        if(playerCombat != null) playerCombat.enabled = enabled;
        if(playerDust != null) playerDust.enabled = enabled;
    }

    void SetAllObjActive(bool value){
        if(obj == null || obj.Length <= 0) return;
        foreach(var o in obj){
            if(o == null) continue;
            o.SetActive(value);
        }
    }
}