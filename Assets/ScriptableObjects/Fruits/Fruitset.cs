using UnityEngine;

[CreateAssetMenu(fileName = "FruitSet", menuName = "Fruit/FruitSet", order = 0)]
public class FruitSet : ScriptableObject
{
    [System.Serializable]
    public struct Entry {
        [Range(1, 10)] public int level;
        public Color color;
        public float radius;        // world units (collider radius)
        public float mass;          // Rigidbody2D.mass
        public int score;           // merge 시 가산 점수
    }

    public Entry[] entries;

    public bool TryGetByLevel(int level, out Entry e) {
        foreach (var x in entries) if (x.level == level) { e = x; return true; }
        e = default; return false;
    }

    public int MinLevel => entries != null && entries.Length > 0 ? entries[0].level : 1;
    public int MaxLevel {
        get {
            int m = int.MinValue;
            foreach (var x in entries) m = Mathf.Max(m, x.level);
            return m == int.MinValue ? 1 : m;
        }
    }
}