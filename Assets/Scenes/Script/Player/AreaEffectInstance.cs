using UnityEngine;
using UnityEngine.AI;

public class AuraDebuffScanner : MonoBehaviour
{
    [Header("Owner")]              // 내 팀
    public NavMeshAgent agent;                // 내 유닛(콜라이더 없어도 됨)

    [Header("Scan")]
    public float extraRange = 2.0f;           // agent 캡슐 반경 + 추가 오라 범위
    public float tickInterval = 0.25f;        // 0.2~0.5 추천
    public LayerMask targetMask;              // 적 유닛 레이어만
    public QueryTriggerInteraction queryTriggers = QueryTriggerInteraction.Ignore;

    [Header("Shape tweak")]
    public float centerYOffset = 0.0f;        // 피벗이 발이면 0~(agent.height*0.5) 조절
    public float radiusScale = 1.0f;
    public float heightScale = 1.0f;

    // NonAlloc 버퍼(인스턴스별로 하나)
    Collider[] hits = new Collider[128];

    float timer;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        timer = Random.value * tickInterval;  // 스캔 타이밍 분산(스파이크 방지)
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer += tickInterval;

        BuildCapsuleFromAgent(agent, out var p1, out var p2, out var r);

        int n = Physics.OverlapCapsuleNonAlloc(
            p1, p2,
            r + extraRange,
            hits,
            targetMask,
            queryTriggers
        );

        for (int i = 0; i < n; i++)
        {
            var col = hits[i];
            if (!col) continue;

            // 콜라이더가 자식에 있어도 루트 유닛 찾기
            var enemy = col.GetComponent<Actor>();
            if (!enemy) continue;
   // 아군 제외
            if (enemy.gameObject == gameObject) continue; // 자기 자신 제외(혹시 콜라이더 붙였을 때)

            // 여기서 “틱마다 갱신” 추천
            enemy.GetComponent<BuffController>().Refresh(DebuffId.Slow, 0.5f);
            // 또는 데미지도 같이:
            // u.TakeDamage(damagePerTick);
        }
    }

    void BuildCapsuleFromAgent(NavMeshAgent a, out Vector3 p1, out Vector3 p2, out float radius)
    {
        float r = a.radius * radiusScale;
        float h = Mathf.Max(a.height * heightScale, r * 2f);

        Vector3 up = transform.up; // 보통 Vector3.up
        Vector3 center = transform.position + up * (h * 0.5f + centerYOffset);

        float half = Mathf.Max(0f, h * 0.5f - r);
        p1 = center + up * half;
        p2 = center - up * half;
        radius = r;
    }
}
