using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Fruit : MonoBehaviour
{
    [SerializeField] private int level;
    public int Level => level;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    public void Init(FruitSet.Entry def) {
        level = def.level;
        var sr = GetComponent<SpriteRenderer>();

        var c = def.color;
        c.a = 1f;
        sr.color = c;

        var rb = GetComponent<Rigidbody2D>();
        rb.mass = Mathf.Max(0.01f, def.mass);
        
        var scale = def.radius / 0.5f;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}