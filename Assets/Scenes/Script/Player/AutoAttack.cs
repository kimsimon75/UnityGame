using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PlayerStats stats;
    ActionScript action;
    HoldScanner hold;
    ItemManager item;
    float attackDelay;
    float hitTiming;
    float Cycle;
    float[] lastAttackTime = new float[ActionScript.targetNumberMax]; // 마지막 공격 시간 기록

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        action = GetComponent<ActionScript>();
        hold = GetComponent<HoldScanner>();
        item = action.item;

        // 초기화
        for (int i = 0; i < ActionScript.targetNumberMax; i++)
        {
            lastAttackTime[i] = 0f;
        }
        Animator animator = GetComponent<Animator>();
        HitPoint hitPointBehaviour = null;

        // Animator Controller의 모든 레이어에서 HitPoint 찾기
        for (int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
        {
            StateMachineBehaviour behaviours = animator.GetBehaviour<HitPoint>();
            if (behaviours != null)
            {
                hitPointBehaviour = (HitPoint)behaviours;
                break;
            }
        }
        hitTiming = hitPointBehaviour.hitTiming;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ActionScript.targetNumberMax; i++)
        {
            if (i == action.targetNumber) continue;
            if (action.isStop[i]) continue;
            
            Cycle = stats.attackCooldown[i];
            attackDelay = stats.attackCooldown[i] * (1 - hitTiming);
            
            if (action.target[i] == null || Vector3.Distance(action.target[i].position, transform.position) > stats.detectRange)
            {
                hold.FindClosestEnemy(transform.position, stats.detectRange, LayerMask.GetMask("Enemy"), i);
            }

            if (action.target[i] != null)
            {
                // ★ Attack 상태가 아닐 때에만 전환
                if (action.IsAttackDisabledFor(i)) continue;
                
                // Cycle 간격으로 공격 실행 체크
                if (Time.time >= lastAttackTime[i] + Cycle)
                {
                    if (action.target[i].gameObject.activeInHierarchy)
                    {
                        // 공격 실행
                        stats.HealthTrigger();
                        stats.ManaTrigger();
                        action.target[i].GetComponent<Actor>()
                            .TakeDamageAll(0, stats.damage[i], 0, ArmorType.패기, true, stats.neutralizeDefense);

                        if (item == null) Debug.Log("None item");
                        foreach (KeyValuePair<(string, ItemRank), Item> kvp in item.list.currentItem[i])
                            item.list.SetSkill(action.target[i].GetComponent<Actor>(), kvp.Value);
                        item.Clear(item.editItem, false);
                        
                        // 마지막 공격 시간 업데이트
                        lastAttackTime[i] = Time.time;
                        // 공격 비활성화 시간 설정 (hitTiming 적용)
                        action.attackDisableTime[i] = Time.time + attackDelay;
                    }
                }
            }
        }
    }
}