using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private FruitFactory factory;
    [SerializeField] private Transform fruitParent; // 생성할 부모 Transform

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnMergedFruit(int level, Vector3 pos)
    {
        if (factory == null) return;

        var fruit = factory.SpawnFruit(pos, level);
        if (fruit != null)
        {
            fruit.Pop();
        }
    }
}
