using UnityEngine;

public enum GameState
{
    Ready,
    Dropping,
    Settling,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public GameState State { get; private set; } = GameState.Ready;

    void OnEnable()
    {
        // 관련 event 구독
    }

    void OnDisable()
    {
        // 관련 event 구독 해제
    }

    public void StartGame()
    {
        State = GameState.Dropping;
        
    }
}
