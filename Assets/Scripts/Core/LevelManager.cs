using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct LevelData
{
    public int minCount;
    public int maxCount;
    public int minLevel;
    public int maxLevel;

    public LevelData(int minCount, int maxCount, int minLevel, int maxLevel)
    {
        this.minCount = minCount;
        this.maxCount = maxCount;
        this.minLevel = minLevel;
        this.maxLevel = maxLevel;
    }

    public bool InRange(int count) => count >= minCount && count <= maxCount;
}

public class LevelManager : MonoBehaviour
{
    private readonly List<LevelData> levels = new List<LevelData>
    {
        new LevelData(0, 4, 1, 3),
        new LevelData(5, 9, 1, 4),
        new LevelData(10, 14, 1, 5),
        new LevelData(15, 19, 1, 6),
        new LevelData(20, 999999, 1, 7)
    };

    public (int minLevel, int maxLevel) GetLevelData(int count)
    {
        foreach (var level in levels)
        {
            if (level.InRange(count))
                return (level.minLevel, level.maxLevel);
        }

        throw new Exception("Invalid Count Data");
    }
}