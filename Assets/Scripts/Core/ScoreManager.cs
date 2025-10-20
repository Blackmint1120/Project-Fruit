using UnityEngine;
using TMPro; // TMP UI 사용 시

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText;
    private int score = 0;

    private int bestScore;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        RefreshUI();
    }

    public void AddScore(int fruitLevel)
    {
        score += fruitLevel * 10;
        if (score > bestScore) {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
        RefreshUI();
    }

    public void ResetScore()
    {
        score = 0;
        RefreshUI();
    }

    void RefreshUI()
    {
        if (scoreText)
            scoreText.text = $"SCORE: {score}\nBEST: {bestScore}";
    }
}