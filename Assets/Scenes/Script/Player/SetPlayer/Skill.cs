using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class Skill : MonoBehaviour
{
    private Camera cam;
    private NavMeshAgent agent;
    private SkillManager skillManager;
    private OriginStatFor6 originStatFor6;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (!cam) cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();  
    }
    void Start()
    {
        skillManager = GetComponent<SkillManager>();
        originStatFor6 = GameManager.Instance.originStatFor6;

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Teleport(Vector3 point)
    {
        float maxRange = DataManager.Instance.RoundX(originStatFor6.GetComponent<SkillManager>().ResolveBlinkRange(originStatFor6.targetNumber) * 0.01f, 3);

        Vector3 from = transform.position;

        // ✅ 사거리 제한은 XZ 기준으로만
        Vector3 planarDelta = new Vector3(point.x - from.x, 0f, point.z - from.z);
        Vector3 planarClamped = Vector3.ClampMagnitude(planarDelta, maxRange);

        // ✅ y는 "클릭한 표면" 높이를 그대로 살림 (여기가 네 코드에서 깨져있던 부분)
        Vector3 candidate = new Vector3(from.x + planarClamped.x, point.y, from.z + planarClamped.z);

        // ✅ 반경 5f는 너무 큼 → 아래층/엉뚱한 NavMesh로 스냅될 가능성 커짐
        const float sampleRadius = 0.75f;

        int areaMask = agent != null ? agent.areaMask : NavMesh.AllAreas;

        if (NavMesh.SamplePosition(candidate, out NavMeshHit navHit, sampleRadius, areaMask))
        {
            // (선택) 클릭한 높이와 너무 다르면 거부하고 싶을 때:
            // if (Mathf.Abs(navHit.position.y - point.y) > 0.6f) return;

            agent.Warp(navHit.position);
            agent.ResetPath();
            agent.velocity = Vector3.zero;

            int q = (int)DataManager.Num.Q;

            originStatFor6.GetComponent<SkillManager>().TryUse(originStatFor6.targetNumber, (SkillId)q);
            
            GameManager.Instance.playerStats[originStatFor6.targetNumber].TeleportOn = false;
        }
    }
}
