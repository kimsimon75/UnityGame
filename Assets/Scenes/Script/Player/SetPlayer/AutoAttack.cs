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
    OriginStatFor6 originStatFor6;
    float attackDelay;
    float hitTiming;
    float Cycle;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        action = GetComponent<ActionScript>();
        hold = GetComponent<HoldScanner>();
        originStatFor6 = GameManager.Instance.originStatFor6;
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
        for (int i = 0; i < DataManager.targetNumberMax; i++)
        {
            if (i == GameManager.Instance.originStatFor6.targetNumber) continue;
            if (action.isStop[i]) continue;

            Cycle = originStatFor6.playerStats[i].attackCooldown;
            attackDelay = originStatFor6.playerStats[i].attackCooldown * (1 - hitTiming);
            

            if (action.target == null || Vector3.Distance(action.target.position, transform.position) > stats.detectRange)
            {
                hold.FindClosestEnemy(transform.position, stats.detectRange, LayerMask.GetMask("Enemy"), i);
            }

            if (action.target != null)
            {

                // Cycle 간격으로 공격 실행 체크
                if (Time.time >= action.attackDisableTime + attackDelay)
                {
                    if (action.target.gameObject.activeInHierarchy)
                    {
                        // 공격 실행
                        stats.HealthTrigger(action.target);
                        stats.ManaTrigger(action.target);
                        originStatFor6.playerStats[i].CurrentHealth += 1;
                        originStatFor6.playerStats[i].CurrentMana += 1;
                        float damageUp = 0;
                        foreach (Item item1 in item.list.currentItem[i])
                            damageUp += item.list.SetSkill(action.target.GetComponent<Actor>(), item1);


                        Actor actor = action.target.GetComponent<Actor>();

                        actor.TakeDamageAll_physics((int)(originStatFor6.playerStats[i].damage * (1 + damageUp + originStatFor6.playerStats[i].damageBonus)), 
                        0, originStatFor6.playerStats[i].Radius, stats.armorType, originStatFor6.playerStats[i].doublePhysics);

                        actor.TakeDamageAll_magics(
                            (int)(originStatFor6.playerStats[i].damage * (1 + damageUp + originStatFor6.playerStats[i].damageBonus) * originStatFor6.playerStats[i].TrueDamage),
                        0, originStatFor6.playerStats[i].Radius, true); 

                        if (item == null) Debug.Log("None item");
                        // 공격 비활성화 시간 설정 (hitTiming 적용)
                        action.attackDisableTime = Time.time + Cycle - attackDelay;
                    }
                }
            }
            else
            {
                action.attackDisableTime = Time.time;
            }
        }
    }
}