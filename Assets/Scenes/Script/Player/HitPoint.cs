using System;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class HitPoint : StateMachineBehaviour
{

    private int lastLoop = -1;
    public bool hashitThisLoop = false;
    private ActionScript action;
    private PlayerStats stats;
    private ItemManager item;
    private GameManager game;
    public AutoAttack autoAttack;
    private float attackDelay;
    public float hitTiming = .45f;
    private float duration = .15f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastLoop = -1;
        hashitThisLoop = false;
        if (action == null) action = animator.GetComponent<ActionScript>();
        if (stats == null) stats = animator.GetComponent<PlayerStats>();
        if (game == null) game = stats.GetComponentInParent<GameManager>();
        item = game.ItemManager;

        float animDuration = stats.attackCooldown[action.targetNumber];

        attackDelay = animDuration * (1 - hitTiming);

        action.attackDisableTime[action.targetNumber] = Time.time + attackDelay;

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (action.IsAttackDisabledFor(action.targetNumber)) return;

        // 1) 전이 중이면 '우리가 붙어있는 상태가 Next인지' 확인하고, 맞으면 nextInfo로 교체
        AnimatorStateInfo info = stateInfo; // 기본은 전달받은 stateInfo
        if (animator.IsInTransition(layerIndex))
        {
            var next = animator.GetNextAnimatorStateInfo(layerIndex);
            var cur  = animator.GetCurrentAnimatorStateInfo(layerIndex);

            // 이 SMB가 붙은 상태와 일치하는 쪽을 '활성 info'로 채택
            if (next.fullPathHash == stateInfo.fullPathHash)
                info = next;   // 들어가는 중 → next 진행도 사용
            else if (cur.fullPathHash == stateInfo.fullPathHash)
                info = cur;    // 나가는 중 → current 진행도 사용
            else
                return;        // (드물지만) 둘 다 아니면 스킵
        }

        float progress = info.normalizedTime % 1f;
        int loop = Mathf.FloorToInt(info.normalizedTime);
        if (loop > lastLoop) { lastLoop = loop; hashitThisLoop = false; }

        if (!hashitThisLoop && progress >= hitTiming)
        {
            if (action.target[action.targetNumber] == null) return;

            // ⚠ LightningBoltScript 사용법 수정: Start/EndObject를 '대상 오브젝트'로 지정하고
            // 오프셋은 Start/EndPosition으로 줘야 null 에러가 안 납니다.
            var lb = action.Clone.GetComponent<LightningBoltScript>();
            lb.EndPosition   = new Vector3(0, 1, 0) + action.target[action.targetNumber].transform.position;

            // 수명은 타이머로 처리 (애니메이터와 독립, 지연 없음)
            var killer = action.Clone.GetComponent<KillMyself>();
            if (killer == null) killer = action.Clone.AddComponent<KillMyself>();
            killer.Init(duration);   // duration = 0.15f 등

            action.Clone.SetActive(true);
            action.Clone.GetComponent<KillMyself>().info = 0;
            // 데미지/스킬
            if (action.target[action.targetNumber].gameObject.activeInHierarchy)
            {
                stats.HealthTrigger();
                stats.ManaTrigger();
                action.target[action.targetNumber].GetComponent<Actor>()
                    .TakeDamageAll(0, stats.damage[action.targetNumber], 0, ArmorType.패기, true, stats.neutralizeDefense);

                if (item == null) Debug.Log("None item");
                foreach (KeyValuePair<(string, ItemRank), Item> kvp in item.list.currentItem[action.targetNumber])
                    item.list.SetSkill(action.target[action.targetNumber].GetComponent<Actor>(), kvp.Value);
                item.Clear(item.editItem, false);
            }

            hashitThisLoop = true;
            action.attackDisableTime[action.targetNumber] = Time.time + attackDelay;
        }
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hashitThisLoop)
            action.attackDisableTime[action.targetNumber] = Time.time + stats.attackCooldown[action.targetNumber] * (1 - stateInfo.normalizedTime % 1f);
        else
            action.attackDisableTime[action.targetNumber] = 0;
        action.Clone.SetActive(false);
    }
}
