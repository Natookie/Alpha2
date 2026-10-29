using UnityEngine;
using NaughtyAttributes;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("MUSIC")]
    [SerializeField] private AudioSource musicSource;
    [Space(10)]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [Space(10)]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private SfxLibrary sfxLibrary;

    public float MusicVolume => musicVolume;

    void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyMusicVolume();
        }else{
            Destroy(gameObject);
            return;
        }
    }

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayGameplayMusic() => PlayMusic(gameplayMusic);
    public void PlayGameOverMusic() => PlayMusic(gameOverMusic);

    void PlayMusic(AudioClip clip){
        if(musicSource == null || clip == null) return;
        if(musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void SetMusicVolume(float value){
        musicVolume = Mathf.Clamp01(value);
        ApplyMusicVolume();
    }

    void ApplyMusicVolume(){
        if(musicSource == null) return;
        musicSource.volume = musicVolume;
    }

    public void PlaySFX(string key, float volumeMultiplier = 1f){
        if(sfxSource == null || sfxLibrary == null) return;

        SfxLibrary.SfxEntry entry = sfxLibrary.Get(key);
        if(entry == null || entry.clip == null) return;

        sfxSource.pitch = entry.GetPitch();
        sfxSource.PlayOneShot(entry.clip, entry.volume * volumeMultiplier);
    }

    public void PlaySFXAt(string key, Vector2 position, float volumeMultiplier = 1f){
        if(sfxLibrary == null) return;

        SfxLibrary.SfxEntry entry = sfxLibrary.Get(key);
        if(entry == null || entry.clip == null) return;

        AudioSource.PlayClipAtPoint(
            entry.clip,
            position,
            entry.volume * volumeMultiplier
        );
    }
}