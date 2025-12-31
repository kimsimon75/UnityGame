using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AuraDebuffScanner : MonoBehaviour
{
    [Header("Owner")]              // 내 팀
    NavMeshAgent agent;                // 내 유닛(콜라이더 없어도 됨)

    [Header("Scan")]
    float Range = 5f;           // agent 캡슐 반경 + 추가 오라 범위
    float tickInterval = 0.25f;        // 0.2~0.5 추천
    LayerMask targetMask;              // 적 유닛 레이어만
    QueryTriggerInteraction queryTriggers = QueryTriggerInteraction.Ignore;

    [Header("Shape tweak")]
    float centerYOffset = 0.0f;        // 피벗이 발이면 0~(agent.height*0.5) 조절

    // NonAlloc 버퍼(인스턴스별로 하나)
    Collider[] hits = new Collider[128];
    PriorityQueue<Item> debuffItem;

    float timer;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        timer = Random.value * tickInterval;  // 스캔 타이밍 분산(스파이크 방지)
        targetMask = LayerMask.GetMask("Enemy");
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer += tickInterval;

        BuildCenterSphereFromAgent(agent, out var p1, out var p2);

        int n = Physics.OverlapCapsuleNonAlloc(
            p1, p2,
            Range,
            hits,
            targetMask,
            queryTriggers
        );

        debuffItem = GameManager.Instance.ItemManager.list.DebuffItem;

        for (int i = 0; i < n; i++)
        {
            var col = hits[i];
                Debug.Log("Here");
            if (!col) continue;

            // 콜라이더가 자식에 있어도 루트 유닛 찾기
            var buffController = GetComponent<BuffController>();
            if (!buffController) continue;
   // 아군 제외
            if (buffController.gameObject == gameObject) continue; // 자기 자신 제외(혹시 콜라이더 붙였을 때)

            // 여기서 “틱마다 갱신” 추천
            foreach(Item item in debuffItem)
            {
                if(item.MoveSpeed != 0)
                    buffController.RefreshSlow(item, BuffType.Slow, item.MoveSpeed, 0.5f);
                if(item.NeutralizeDefense != 0)
                    buffController.RefreshSlow(item, BuffType.ArmorDecrease, item.NeutralizeDefense, 0.5f);
                
            }
            // 또는 데미지도 같이:
            // u.TakeDamage(damagePerTick);
        }
    }

    void BuildCenterSphereFromAgent(NavMeshAgent a, out Vector3 p1, out Vector3 p2)
    {
        Vector3 up = transform.up;
        Vector3 center = transform.position + up * (a.baseOffset + a.height * 0.5f + centerYOffset);

        p1 = center;
        p2 = center;
    }
}
