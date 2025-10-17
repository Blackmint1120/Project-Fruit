using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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
    private bool isLocked = false;

    readonly Queue<int> _queue = new();

    float _currentRadius = 0.5f;    // 현재 드롭 예정 과일의 반지름
    float _bottomY;                 // 바닥선 y (Walls에 맞춰 세팅 권장)

    void Start()
    {
        if (!mainCam) mainCam = Camera.main;
        SeedQueue();

        // 맨 처음 반지름 캐시
        UpdateCurrentRadius();

        // 바닥선 찾기(없으면 임시로 화면 하단)
        _bottomY = FindFloorY() ?? (mainCam.transform.position.y - mainCam.orthographicSize + 0.5f);

        if (guide)
        {
            guide.enabled = useGuideLine;
            if (useGuideLine) guide.positionCount = 2;
        }
    }

    void Update()
    {
        Vector3 world = GetWorldPointerOnSpawnY();
        world.x = Mathf.Clamp(world.x, -clampX + _currentRadius, clampX - _currentRadius);

        // 가이드라인만 업데이트
        if (useGuideLine && guide)
        {
            guide.enabled = true;
            guide.SetPosition(0, new Vector3(world.x, spawnY.position.y, 0f)); // 위
            guide.SetPosition(1, new Vector3(world.x, _bottomY, 0f));          // 아래
        }

        // 드롭 입력
        if (Input.GetKeyDown(KeyCode.Space) && !isLocked)
        {
            Debug.Log("Space pressed, starting coroutine!");
            StartCoroutine(LockDrop(world));
        }
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

    Vector3 GetWorldPointerOnSpawnY()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 world = mainCam.ScreenToWorldPoint(mouse);
        world.y = spawnY.position.y;
        world.z = 0;
        return world;
    }

    float? FindFloorY()
    {
        // 선택: Walls(바닥) 오브젝트를 찾아서 y를 얻고 싶다면 태그/레이어로 탐색
        // 간단히 바닥 콜라이더를 Find 하거나, GameRoot에서 참조 받아도 OK.
        return null;
    }

    IEnumerator LockDrop(Vector3 world)
    {
        isLocked = true;
        DropAt(world.x);
        yield return new WaitForSeconds(dropLock);
        isLocked = false;
    }
}
