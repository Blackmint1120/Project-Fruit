using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    [SerializeField] private float triggerTime = 1.5f; // 일정 시간 유지 시 게임오버
    private float timer = 0f;

    void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Fruit")) return;
        timer += Time.deltaTime;
        
        Debug.Log("Gameoverzone detected : time = " + timer);
        
        if (timer >= triggerTime)
        {
            GameManager.Instance.GameOver();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Fruit")) return;
        timer = 0f; // 빠져나가면 초기화
    }
}