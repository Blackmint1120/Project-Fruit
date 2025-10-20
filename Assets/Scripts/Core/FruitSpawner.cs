using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;

public class FruitSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private FruitFactory factory;
    [SerializeField] private Transform spawnY;      // 상단 y 기준
    [SerializeField] private LineRenderer guide;    // 가이드라인 참조 (필수)

    [Header("Queue")]
    [SerializeField] private int previewCount = 2;
    [SerializeField] private int minLevel = 1;
    [SerializeField] private int maxLevel = 6;

    [Header("Options")]
    [SerializeField] private bool useGuideLine = true;
    [SerializeField] private float clampX = 5f; // 카메라 비율에 맞게 조정
    [SerializeField] private float spawnYOffset = 0.05f; // 라인보다 살짝 아래로
    public float dropLock = 1f;
    
    [Header("UI Preview")]
    [SerializeField] private Image nextFruitImage;   // 🔹 다음 과일 표시용 UI
    [SerializeField] private float nextFruitScale = 1.0f;
    
    private bool isLocked = false;

    private readonly Queue<int> _queue = new();

    float _currentRadius = 0.5f;    // 현재 드롭 예정 과일의 반지름
    float _bottomY;                 // 바닥선 y (Walls에 맞춰 세팅 권장)
    
    private Fruit currentFruit;

    void Start()
    {
        if (!mainCam) mainCam = Camera.main;
        SeedQueue();

        // 맨 처음 반지름 캐시
        UpdateCurrentRadius();

        // 바닥선 찾기(없으면 임시로 화면 하단)
        _bottomY = FindFloorY();
        if (guide)
        {
            guide.enabled = useGuideLine;
            if (useGuideLine) guide.positionCount = 2;
        }
        
        SpawnNextFruit();
    }

    void Update()
    {
        if (currentFruit)
            UpdateCurrentFruitPosition();
        
        if (useGuideLine && guide)
            UpdateGuideLine();

        if (Input.GetKeyDown(KeyCode.Space) && !isLocked) 
            StartCoroutine(LockDrop());
    }

    void SeedQueue()
    {
        int min = factory.FruitSet.MinLevel;
        int max = factory.FruitSet.MaxLevel;
        if (minLevel < min) minLevel = min;
        if (maxLevel > max) maxLevel = max;

        while (_queue.Count < previewCount + 1)
            _queue.Enqueue(Random.Range(minLevel, maxLevel + 1));
    }

    int PeekCurrent() => _queue.Peek();
    int NextAndRefill()
    {
        int cur = _queue.Dequeue();
        _queue.Enqueue(Random.Range(minLevel, maxLevel + 1));
        return cur;
    }

    void UpdateCurrentRadius()
    {
        int lv = PeekCurrent();
        if (factory.FruitSet.TryGetByLevel(lv, out var def))
            _currentRadius = def.radius;
    }

    void DropAt(float x)
    {
        int lv = NextAndRefill();
        // 갱신된 다음 과일 반지름을 미리 캐시
        // (다음 프레임 클램프/라인에 반영되도록)
        // 먼저 드롭용 위치 계산은 '직전 반지름'으로
        float y = spawnY.position.y - (_currentRadius + spawnYOffset);
        var fruit = factory.SpawnFruit(new Vector2(x, y), lv);
        if (fruit) fruit.Pop();

        // 큐가 바뀌었으니 현재 반지름 갱신
        UpdateCurrentRadius();
    }
    
    void SpawnNextFruit()
    {
        int lv = PeekCurrent();
        float y = spawnY.position.y - (_currentRadius + spawnYOffset);
        var f = factory.SpawnFruit(new Vector2(0, y), lv);
        if (!f) return;

        var rb = f.GetComponent<Rigidbody2D>();
        var col = f.GetComponent<Collider2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        col.enabled = false;

        currentFruit = f;
        
        UpdateNextFruitPreview();
    }

    void DropCurrentFruit()
    {
        if (!currentFruit) return;

        var rb = currentFruit.GetComponent<Rigidbody2D>();
        var col = currentFruit.GetComponent<Collider2D>();
        rb.gravityScale = 1f;
        col.enabled = true;

        currentFruit.Pop();

        currentFruit = null;

        // 큐 갱신
        NextAndRefill();
        UpdateNextFruitPreview();
        UpdateCurrentRadius();

        // 다음 과일 준비
        StartCoroutine(SpawnNextAfterDelay(0.3f));
    }

    IEnumerator SpawnNextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNextFruit();
    }

    IEnumerator LockDrop()
    {
        isLocked = true;
        DropCurrentFruit();
        yield return new WaitForSeconds(dropLock);
        isLocked = false;
    }

    void UpdateCurrentFruitPosition()
    {
        Vector3 world = GetWorldPointerOnSpawnY();
        float clampedX = Mathf.Clamp(world.x, -clampX + _currentRadius, clampX - _currentRadius);

        Vector3 pos = currentFruit.transform.position;
        pos.x = clampedX;
        currentFruit.transform.position = pos;
    }

    // -------------------------------
    // Line & Helpers
    // -------------------------------
    void UpdateGuideLine()
    {
        Vector3 world = GetWorldPointerOnSpawnY();
        float clampedX = Mathf.Clamp(world.x, -clampX + _currentRadius, clampX - _currentRadius);

        guide.enabled = true;
        guide.SetPosition(0, new Vector3(clampedX, spawnY.position.y, 0f));
        guide.SetPosition(1, new Vector3(clampedX, _bottomY, 0f));
    }

    Vector3 GetWorldPointerOnSpawnY()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 world = mainCam.ScreenToWorldPoint(mouse);
        world.y = spawnY.position.y;
        world.z = 0;
        return world;
    }

    float FindFloorY()
    {
        Vector2 start = spawnY.position;
        RaycastHit2D hit = Physics2D.Raycast(start, Vector2.down);
        return hit ? hit.point.y : spawnY.position.y - 10f;
    }

    // -------------------------------
    // GuideLine Toggle
    // -------------------------------
    public void OnToggleGuideLine(bool on)
    {
        useGuideLine = on;
        if (guide)
        {
            guide.enabled = on;
            if (on)
                UpdateGuideLine();
        }
    }
    
    void UpdateNextFruitPreview()
    {
        if (!nextFruitImage) return;

        int nextLevel = GetNextInQueue(1);
        if (factory.FruitSet.TryGetByLevel(nextLevel, out var def))
        {
            // circleSprite 하나로 색상만 다르게 표시
            nextFruitImage.sprite = factory.SharedCircleSprite;
            nextFruitImage.color = def.color;
        }
    }
    
    int GetNextInQueue(int index)
    {
        if (_queue.Count == 0) return minLevel;
        index = Mathf.Clamp(index, 0, _queue.Count - 1);

        int i = 0;
        foreach (var lv in _queue)
        {
            if (i == index) return lv;
            i++;
        }
        return _queue.Peek();
    }
}
