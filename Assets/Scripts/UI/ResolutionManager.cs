using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public static ResolutionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // scene 이동해도 계속 활용
        
        ApplyDefaultResolution();
    }
    
    private void Update()
    {
        // Alt+Enter로 전체화면 전환 토글 예시
        if (Input.GetKeyDown(KeyCode.Return) && Input.GetKey(KeyCode.LeftAlt))
        {
            ToggleFullscreen();
        }
    }
    
    private void ToggleFullscreen()
    {
        var isFull = Screen.fullScreen;
        if (isFull)
        {
            Screen.SetResolution(1280, 720, false);
        }
        else
        {
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        }
    }
    
    private void ApplyDefaultResolution()
    {
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
    }
}