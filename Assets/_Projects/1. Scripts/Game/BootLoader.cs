using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GAME";
    void Start() => SceneManager.LoadScene(gameSceneName, LoadSceneMode.Additive);
}