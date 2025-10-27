using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FruitSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private FruitFactory factory;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Transform spawnY;
    [SerializeField] private LineRenderer guide;

    [Header("Level")] 
    [SerializeField] private int fruitCounter = 0;

    [Header("Options")]
    [SerializeField] private bool useGuideLine = true;
    [SerializeField] private float clampX = 5f;
    [SerializeField] private float spawnYOffset = 0.05f;
    public float dropLock = 1f;
    
    [Header("UI Preview")]
    [SerializeField] private Image nextFruitImage;
    
    private bool isLocked = false;

    private readonly Queue<int> _queue = new();

    private float _currentRadius = 0.5f;
    private float _bottomY;
    
    private Fruit currentFruit;

    private void Start()
    {
        if (!mainCam) mainCam = Camera.main;

        // 다음에 나올 Fruit 표시 위해 처음에는 2개 enqueue
        for (var i = 0; i < 2; i++)
        {
            FruitEnqueue();
        }

        // 맨 처음 반지름 캐시
        UpdateCurrentRadius();

        // 바닥선 찾기
        _bottomY = FindFloorY();
        if (guide)
        {
            guide.enabled = useGuideLine;
        }
        
        SpawnNextFruit();
    }

    private void Update()
    {
        if (currentFruit&& gameManager.State == GameState.Playing)
            UpdateCurrentFruitPosition();
        
        if (useGuideLine && guide && gameManager.State == GameState.Playing)
            UpdateGuideLine();

        if (Input.GetKeyDown(KeyCode.Space) && !isLocked && gameManager.State == GameState.Playing) 
            StartCoroutine(LockDrop());
    }

    private void FruitEnqueue()
    {
        fruitCounter++;
        (var minLevel, var maxLevel) = levelManager.GetLevelData(fruitCounter);
        _queue.Enqueue(Random.Range(minLevel, maxLevel + 1));
    }

    private int PeekCurrent() => _queue.Peek();
    private void NextAndRefill()
    {
        _ = _queue.Dequeue();
        FruitEnqueue();
    }

    private void UpdateCurrentRadius()
    {
        var lv = PeekCurrent();
        if (factory.FruitSet.TryGetByLevel(lv, out var def))
            _currentRadius = def.radius;
    }
    
    private void SpawnNextFruit()
    {
        var lv = PeekCurrent();
        var y = spawnY.position.y - (_currentRadius + spawnYOffset);
        var f = factory.SpawnFruit(new Vector2(0, y), lv);
        
        if (!f) throw new Exception("SpawnNextFruit : fruit spawn failed");

        var rb = f.GetComponent<Rigidbody2D>();
        var col = f.GetComponent<Collider2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        col.enabled = false;

        currentFruit = f;
        
        UpdateNextFruitPreview();
    }

    private void DropCurrentFruit()
    {
        if (!currentFruit) throw new Exception("DropCurrentFruit : current fruit is null");

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

    private IEnumerator SpawnNextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNextFruit();
    }

    private IEnumerator LockDrop()
    {
        isLocked = true;
        DropCurrentFruit();
        yield return new WaitForSeconds(dropLock);
        isLocked = false;
    }

    private void UpdateCurrentFruitPosition()
    {
        var world = GetWorldPointerOnSpawnY();
        var clampedX = Mathf.Clamp(world.x, -clampX + _currentRadius, clampX - _currentRadius);

        var pos = currentFruit.transform.position;
        pos.x = clampedX;
        currentFruit.transform.position = pos;
    }

    // -------------------------------
    // GuideLine
    // -------------------------------
    private void UpdateGuideLine()
    {
        var world = GetWorldPointerOnSpawnY();
        var clampedX = Mathf.Clamp(world.x, -clampX + _currentRadius, clampX - _currentRadius);

        guide.enabled = true;
        guide.SetPosition(0, new Vector3(clampedX, spawnY.position.y, 0f));
        guide.SetPosition(1, new Vector3(clampedX, _bottomY, 0f));
    }

    private Vector3 GetWorldPointerOnSpawnY()
    {
        var mouse = Input.mousePosition;
        mouse.z = Mathf.Abs(mainCam.transform.position.z);
        var world = mainCam.ScreenToWorldPoint(mouse);
        world.y = spawnY.position.y;
        world.z = 0;
        return world;
    }

    private float FindFloorY()
    {
        Vector2 start = spawnY.position;
        var hit = Physics2D.Raycast(start, Vector2.down);
        if (!hit) throw new Exception("Cannot find floor");
        return hit.point.y;
    }

    // -------------------------------
    // GuideLine Toggle
    // -------------------------------
    public void OnToggleGuideLine(bool on)
    {
        useGuideLine = on;
        if (!guide) return;
        
        guide.enabled = on;
        if (on)
            UpdateGuideLine();
    }
    
    private void UpdateNextFruitPreview()
    {
        if (!nextFruitImage) throw new Exception("UpdateNextFruitPreview: next fruit image is null");

        var nextLevel = GetNextInQueue(1);
        if (factory.FruitSet.TryGetByLevel(nextLevel, out var def))
        {
            // circleSprite 하나로 색상만 다르게 표시
            nextFruitImage.sprite = factory.SharedCircleSprite;
            nextFruitImage.color = def.color;
        }
    }
    
    private int GetNextInQueue(int index)
    {
        if (_queue.Count == 0) throw new Exception("Queue is empty");
        index = Mathf.Clamp(index, 0, _queue.Count - 1);

        var i = 0;
        foreach (var lv in _queue)
        {
            if (i == index) return lv;
            i++;
        }
        return _queue.Peek();
    }
}
