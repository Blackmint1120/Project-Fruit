using System;
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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void Start()
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
        switch (State)
        {
            case GameState.Playing:
                State = GameState.Paused;
                Time.timeScale = 0f;
                if (pausePanel) pausePanel.SetActive(true);
                break;
            case GameState.GameOver:
                break;
            case GameState.Paused:
                Resume();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Resume()
    {
        if (State != GameState.Paused) throw new Exception($"Resume in Invalid state : {State}");
        
        State = GameState.Playing;
        Time.timeScale = 1f;
        if (pausePanel) pausePanel.SetActive(false);
    }
    
    public void RestartGame()
    {
        if (State != GameState.Paused && State != GameState.GameOver) throw new Exception($"Restart in Invalid state : {State}");
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }
    
    public void GoToTitle()
    {
        if (State != GameState.Paused && State != GameState.GameOver) throw new Exception($"GoToTitle in Invalid state : {State}");
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void SpawnMergedFruit(int level, Vector3 pos)
    {
        if (factory == null) return;

        factory.SpawnFruit(pos, level);
    }
}
