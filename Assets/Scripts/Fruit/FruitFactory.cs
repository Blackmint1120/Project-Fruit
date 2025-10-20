using UnityEngine;

public class FruitFactory : MonoBehaviour
{
    [SerializeField] private FruitSet fruitSet;
    [SerializeField] private GameObject fruitPrefab; // 빈 오브젝트에 SpriteRenderer/Rigidbody2D/CircleCollider2D
    public FruitSet FruitSet => fruitSet;
    
    [SerializeField] private Sprite circleSprite;
    public Sprite SharedCircleSprite => circleSprite;

    public Fruit SpawnFruit(Vector2 pos, int level)
    {
        if (!fruitSet.TryGetByLevel(level, out var def)) {
            Debug.LogError($"No fruit def for level {level}"); return null;
        }
        var go = Instantiate(fruitPrefab, pos, Quaternion.identity);
        var f = go.GetComponent<Fruit>();
        f.Init(def);
        return f;
    }
}