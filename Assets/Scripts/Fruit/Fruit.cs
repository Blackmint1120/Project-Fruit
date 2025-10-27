using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Fruit : MonoBehaviour
{
    [SerializeField] private int level;
    public int Level => level;

    void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    public void Init(FruitSet.Entry def) {
        level = def.level;
        var sr = GetComponent<SpriteRenderer>();

        // 스프라이트는 기본 원(circle) 사용, 색상만 변경
        if (sr.sprite == null)
            sr.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
        // 또는 직접 circle.png 한 장 만들어 넣어도 OK
    
        var c = def.color;
        c.a = 1f;
        sr.color = c;// 완전 불투명 보장

        var rb = GetComponent<Rigidbody2D>();
        rb.mass = Mathf.Max(0.01f, def.mass);
        
        float baseRadius = 0.5f;
        float scale = def.radius / baseRadius;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}