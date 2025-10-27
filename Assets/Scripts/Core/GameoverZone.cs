using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    [SerializeField] private float triggerTime = 3f; // 일정 시간 유지 시 게임오버
    private float timer = 0f;

    private int timerlog = 0;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Fruit")) return;
        timer += Time.deltaTime;

        if (timer >= timerlog + 1)
        {
            Debug.Log($"{3 - timerlog}...");
        }

        if (timer < triggerTime) return;
        
        // Game Over
        timerlog = 0; 
        GameManager.Instance.GameOver();
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Fruit")) return;
        timerlog = 0;
        timer = 0f; // 빠져나가면 초기화
    }
}