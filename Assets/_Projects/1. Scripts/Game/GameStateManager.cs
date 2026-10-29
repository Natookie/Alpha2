using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("CONFIG")]
    [SerializeField] private string gameSceneName = "Game";
    [Space(10)]
    [SerializeField] private float playerDeathDelay = 0f;
    [SerializeField] private float bossDeathDelay = 3f;

    [Header("UI")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("REFERENCES")]
    [SerializeField] private PlayerLogic playerLogic;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private BossStats bossStats;
    [SerializeField] private BossLogic bossLogic;

    [Header("DEBUG")]
    [SerializeField, ReadOnly] private GameState currentState = GameState.Menu;

    public GameState CurrentState => currentState;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start() => EnterMenu();

    void OnEnable(){
        if(playerStats != null) playerStats.OnDeath += HandlePlayerDeath;
        if(bossStats != null) bossStats.OnDeath += HandleBossDeath;
    }

    void OnDisable(){
        if(playerStats != null) playerStats.OnDeath -= HandlePlayerDeath;
        if(bossStats != null) bossStats.OnDeath -= HandleBossDeath;
    }

    public void EnterMenu(){
        currentState = GameState.Menu;

        if(menuPanel != null) menuPanel.SetActive(true);
        if(gameOverPanel != null) gameOverPanel.SetActive(false);

        SetPlayerControl(false);
        SetBossActive(false);

        Time.timeScale = 1f;

        AudioManager.Instance?.PlayMenuMusic();
    }

    public void StartGame(){
        currentState = GameState.Playing;

        if(menuPanel != null) menuPanel.SetActive(false);
        if(gameOverPanel != null) gameOverPanel.SetActive(false);

        SetPlayerControl(true);
        SetBossActive(true);

        AudioManager.Instance?.PlayGameplayMusic();
    }

    public void RestartGame(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void HandlePlayerDeath(){
        if(currentState != GameState.Playing) return;
        currentState = GameState.GameOver;
        bossLogic.enabled = false;

        StartCoroutine(GameOverSequence(playerDeathDelay));
    }

    void HandleBossDeath(){
        if(currentState != GameState.Playing) return;
        currentState = GameState.GameOver;
        StartCoroutine(GameOverSequence(bossDeathDelay));
    }

    IEnumerator GameOverSequence(float delay){
        if(delay > 0f){
            yield return new WaitForSecondsRealtime(delay);
        }

        SetPlayerControl(false);
        SetBossActive(false);

        if(gameOverPanel != null) gameOverPanel.SetActive(true);

        AudioManager.Instance?.PlayGameOverMusic();
    }

    void SetPlayerControl(bool enabled){
        if(playerLogic != null) playerLogic.enabled = enabled;
        if(playerCombat != null) playerCombat.enabled = enabled;
    }

    void SetBossActive(bool active){
        if(bossLogic != null) bossLogic.enabled = active;
    }
}