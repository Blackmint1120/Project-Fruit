using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public static ResolutionManager Instance { get; private set; }  // ✅ 전역 접근용 static 인스턴스

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // ✅ 씬 이동 시 파괴되지 않음
        
        ApplyDefaultResolution();
    }
    
    void Update()
    {
        // Alt+Enter로 전체화면 전환 토글 예시
        if (Input.GetKeyDown(KeyCode.Return) && Input.GetKey(KeyCode.LeftAlt))
        {
            ToggleFullscreen();
        }
    }
    
    void ToggleFullscreen()
    {
        bool isFull = Screen.fullScreen;
        if (isFull)
        {
            // ✅ 창모드로 전환 + 원하는 해상도 지정
            Screen.SetResolution(1280, 720, false);
        }
        else
        {
            // ✅ 전체화면으로 전환 (모니터 해상도 자동)
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        }
    }
    
    void ApplyDefaultResolution()
    {
        // 처음 시작 시 창모드 1280x720
        Screen.SetResolution(1280, 720, false);
    }
}