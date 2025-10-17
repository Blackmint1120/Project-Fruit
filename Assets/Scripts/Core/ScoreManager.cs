using UnityEngine;
using TMPro; // TMP UI 사용 시

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText;
    private int score = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() => RefreshUI();

    public void AddScore(int fruitLevel)
    {
        // 간단히 레벨당 10점, 나중에 테이블화 가능
        int gain = fruitLevel * 10;
        score += gain;
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
            scoreText.text = $"SCORE: {score}";
    }
}