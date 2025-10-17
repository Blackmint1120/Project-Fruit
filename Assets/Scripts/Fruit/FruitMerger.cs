using UnityEngine;

[RequireComponent(typeof(Fruit))]
public class FruitMerger : MonoBehaviour
{
    private Fruit fruit;
    private Rigidbody2D rb;
    private bool merged = false;

    [SerializeField] private float mergeCooldown = 0.15f; // 두 번 중복 합침 방지
    private float disableTimer = 0f;

    void Awake()
    {
        fruit = GetComponent<Fruit>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (disableTimer > 0f)
            disableTimer -= Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (disableTimer > 0f) return;
        if (!col.gameObject.CompareTag("Fruit")) return;

        var other = col.gameObject.GetComponent<Fruit>();
        if (!other) return;

        // 같은 레벨만 합치기
        if (other.Level == fruit.Level && !merged && !other.GetComponent<FruitMerger>().merged)
        {
            MergeWith(other);
        }
    }

    void MergeWith(Fruit other)
    {
        merged = true;
        other.GetComponent<FruitMerger>().merged = true;

        // 중간점 계산
        Vector3 mid = (transform.position + other.transform.position) / 2f;

        // 다음 레벨
        int nextLevel = fruit.Level + 1;

        // 팝 이펙트
        GameManager.Instance.SpawnMergedFruit(nextLevel, mid);

        // 점수 추가
        ScoreManager.Instance.AddScore(fruit.Level);

        // 이 둘 삭제
        Destroy(gameObject);
        Destroy(other.gameObject);

        // 쿨다운
        disableTimer = mergeCooldown;
    }
}
