using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private FruitFactory factory;
    [SerializeField] private Transform fruitParent;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    
    public GameState State { get; private set; } = GameState.Playing;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
    }
    
    public void GameOver()
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;
        Time.timeScale = 0f;
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }
    
    public void TogglePause()
    {
        if (State == GameState.GameOver) return;

        if (State == GameState.Playing)
        {
            State = GameState.Paused;
            Time.timeScale = 0f;
            if (pausePanel) pausePanel.SetActive(true);
        }
        else
        {
            Resume();
        }
    }
    
    public void Resume()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        if (pausePanel) pausePanel.SetActive(false);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }
    
    public void GoToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void SpawnMergedFruit(int level, Vector3 pos)
    {
        if (factory == null) return;

        var fruit = factory.SpawnFruit(pos, level);
        if (fruit != null)
        {
            fruit.Pop();
        }
    }
}
