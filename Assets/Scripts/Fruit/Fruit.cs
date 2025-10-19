using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Fruit : MonoBehaviour
{
    [SerializeField] private int level;
    public int Level => level;

    // 머지 직후 작은 팝 연출용
    float popTimer = 0f;
    const float PopDuration = 0.08f;
    Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
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
        
        baseScale = transform.localScale;
    }


    public void Pop() { popTimer = PopDuration; }

    void Update() {
        if (popTimer > 0f) {
            popTimer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(popTimer / PopDuration);
            float s = Mathf.Lerp(1f, 1.1f, t);
            transform.localScale = baseScale * s;
            if (popTimer <= 0f) transform.localScale = baseScale;
        }
    }
}