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
    private float attackDelay;
    public float hitTiming;
    private float duration = .15f;
    
    KillMyself killer ;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastLoop = -1;
        hashitThisLoop = false;

        if (action == null) action = animator.GetComponent<ActionScript>();
        if (stats == null) stats = animator.GetComponent<PlayerStats>();
        if (game == null) game = stats.GetComponentInParent<GameManager>();
        item = game.ItemManager;

        switch(GameManager.Instance.playerCharacter)
        {
            case 0:
                hitTiming = 0.45f;
                break;
            case 1:
                hitTiming = 0.3f;
                break;
        }

        float animDuration = stateInfo.length;

        attackDelay = animDuration * (1 - hitTiming);
        killer = action.Clone.GetComponent<KillMyself>();
        if (killer == null) killer = action.Clone.AddComponent<KillMyself>();



    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (action.IsAttackDisabledFor()) return;

        if (action.target == null)
        {
            action.Clone.SetActive(false);
            animator.CrossFade("Idle", 0.0f, layerIndex); // 네 Idle 상태명으로
            return;
        }

        // 1) 전이 중이면 '우리가 붙어있는 상태가 Next인지' 확인하고, 맞으면 nextInfo로 교체
        AnimatorStateInfo info = stateInfo; // 기본은 전달받은 stateInfo

        float progress = info.normalizedTime % 1f;
        int loop = Mathf.FloorToInt(info.normalizedTime);
        if (loop > lastLoop) { lastLoop = loop; hashitThisLoop = false; }

        if (!hashitThisLoop && progress >= hitTiming)
        {
            if (action.target == null || !action.target.gameObject.activeInHierarchy) return;

            // ⚠ LightningBoltScript 사용법 수정: Start/EndObject를 '대상 오브젝트'로 지정하고
            // 오프셋은 Start/EndPosition으로 줘야 null 에러가 안 납니다.
            var lb = action.Clone.GetComponent<LightningBoltScript>();
            lb.EndObject = action.target.gameObject;

            // 수명은 타이머로 처리 (애니메이터와 독립, 지연 없음)
            killer.Init(duration);   // duration = 0.15f 등

            action.Clone.SetActive(true);
            action.Clone.GetComponent<KillMyself>().info = 0;
            // 데미지/스킬
            if (action.target.gameObject.activeInHierarchy)
            {
                stats.HealthTrigger(action.target);
                stats.ManaTrigger(action.target);
                stats.CurrentHealth += 1;
                stats.CurrentMana += 1;
                float damageUp = 0;
                foreach (Item item1 in item.list.currentItem[GameManager.Instance.originStatFor6.targetNumber])
                    damageUp += item.list.SetSkill(action.target.GetComponent<Actor>(), item1);

                Actor actor = action.target.GetComponent<Actor>();

                actor.TakeDamageAll_physics((int)(stats.damage * (1 + damageUp + stats.damageBonus)), 
                0, stats.Radius, stats.armorType, stats.doublePhysics);

                actor.TakeDamageAll_magics(
                    (int)(stats.damage * (1 + damageUp + stats.damageBonus) * stats.TrueDamage),
                 0, stats.Radius, true);
                
            }

            hashitThisLoop = true;
            action.attackDisableTime = Time.time + attackDelay;
        }
        if (hashitThisLoop)
            SetDisableUntilStateEnd(stateInfo);
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hashitThisLoop)
            action.attackDisableTime = Time.time + stats.attackCooldown * (1 - stateInfo.normalizedTime % 1f);
        else
            action.attackDisableTime = 0;
        action.Clone.SetActive(false);
    }

    private void SetDisableUntilStateEnd(AnimatorStateInfo info)
    {
        float progress = info.normalizedTime % 1f;          // 0~1
        float remaining = (1f - progress) * info.length;    // 지금 속도 기준 남은 초
        action.attackDisableTime = Time.time + remaining;
    }
}
