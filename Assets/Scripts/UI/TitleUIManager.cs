using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button gameStartButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI topScoreText;

    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnStartClicked);
    }

    private void Start()
    {
        topScoreText.text = string.Concat("Top Score : ", PlayerPrefs.GetInt("BestScore", 0).ToString());
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
}