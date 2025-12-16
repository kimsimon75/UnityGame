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

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        action = GetComponent<ActionScript>();
        hold = GetComponent<HoldScanner>();
        item = GameManager.Instance.ItemManager;

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
        for (int i = 0; i < GameManager.Instance.Action.TargetNumberMax; i++)
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

                // Cycle 간격으로 공격 실행 체크
                if (Time.time >= action.attackDisableTime[i] + attackDelay)
                {
                    if (action.target[i].gameObject.activeInHierarchy)
                    {
                        // 공격 실행
                        stats.HealthTrigger(i);
                        stats.ManaTrigger(i);
                        stats.CurrentHealth[i] += 1;
                        stats.CurrentMana[i] += 1;
                        float damageUp = 0;
                        foreach (Item item1 in item.list.currentItem[i])
                            damageUp += item.list.SetSkill(action.target[i].GetComponent<Actor>(), item1);


                        Actor actor = action.target[i].GetComponent<Actor>();

                        actor.TakeDamageAll_physics((int)(stats.damage[i] * (1 + damageUp + stats.damageBonus[i])), 
                        0, stats.Radius[i], stats.armorType, stats.doublePhysics[i], stats.neutralizeDefense);

                        actor.TakeDamageAll_magics(
                            (int)(stats.damage[i] * (1 + damageUp + stats.damageBonus[i]) * stats.TrueDamage[i]),
                        0, stats.Radius[i], true); 

                        if (item == null) Debug.Log("None item");
                        // 공격 비활성화 시간 설정 (hitTiming 적용)
                        action.attackDisableTime[i] = Time.time + Cycle - attackDelay;
                    }
                }
            }
            else
            {
                action.attackDisableTime[i] = Time.time;
            }
        }
    }
}