using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class Skill : MonoBehaviour
{
    private PlayerStats stats;
    private Camera cam;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (!cam) cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();  
    }
    void Start()
    {
        stats = GetComponent<PlayerStats>();

    }

    // Update is called once per frame
    void Update()
    {
        bool anyActive = false;

        for (int i = 0; i < DataManager.NumCount-1; i++) // ✅ -1 제거
        {
            if (stats.someSortOfSkillActive[i] <= 0f)
            {
                stats.someSortOfSkillActive[i] = 0f;
                BuffDelete(i);
                continue;
            }

            stats.someSortOfSkillActive[i] -= Time.deltaTime;

            if (stats.someSortOfSkillActive[i] <= 0f)
            {
                stats.someSortOfSkillActive[i] = 0f;
                BuffDelete(i);
            }
            else
            {
                anyActive = true;
            }
        }

        if (!anyActive)
            enabled = false;
    }

    private void BuffDelete(int target)
    {
        switch ((DataManager.Num)target)
        {
            case DataManager.Num.W:
            for(int i=0;i<GameManager.Instance.Action.TargetNumberMax; i++)
                stats.attackSpeedBonusBonus[i] = 0f;
            break;
            case DataManager.Num.E:
            for(int i=0;i<GameManager.Instance.Action.TargetNumberMax; i++)
                stats.damageBonus[i] = 0;
            break;
        }
    }

    public void ApplyAttackBuff(int target)
    {
        stats.someSortOfSkillActive[target] = stats.someSortOfSkillDuration[target];
        if((DataManager.Num)target == DataManager.Num.W)
        {
            for(int i=0;i<GameManager.Instance.Action.TargetNumberMax; i++)
            {
                stats.attackSpeedBonusBonus[i] = stats.someSortOfSkillEffect[target];
            }
        }
        if((DataManager.Num)target == DataManager.Num.E)
        {
            for(int i=0;i<GameManager.Instance.Action.TargetNumberMax; i++)
            {
                stats.damageBonus[i] = stats.someSortOfSkillEffect[target] * 0.01f;
            }
        }
        stats.someSortOfSkillCooldown[target].Start(stats.someSortOfSkillCooltime[target]);
        enabled = true; 
    }

    public void Teleport(Vector3 point)
    {    
        float maxRange = stats.someSortOfSkillEffect[(int)DataManager.Num.Q]; // ✅ 사거리 따로! (예: 5f)

        Vector3 from = transform.position;

        // 2D(XZ) 기준 사거리 제한
        Vector3 planarDelta = new Vector3(point.x - from.x, 0f, point.z - from.z);
        Vector3 limitedPoint = from + Vector3.ClampMagnitude(planarDelta, maxRange);
        if (NavMesh.SamplePosition(limitedPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);

            int q = (int)DataManager.Num.Q;

            stats.someSortOfSkillCooldown[q].Start(stats.someSortOfSkillCooltime[q]);
            
            GameManager.Instance.TeleportOn = false;
        }
    }
}
