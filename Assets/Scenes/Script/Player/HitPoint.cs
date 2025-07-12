using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;

public class HitPoint : StateMachineBehaviour
{

    private int lastLoop = -1;
    public bool hashitThisLoop = false;
    private ActionScript action;
    private PlayerStats stats;
    private ItemManager item;
    private float attackDelay;
    private float hitTiming = .4f;
    private int currentLoop;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastLoop = -1;
        hashitThisLoop = false;
        action = animator.GetComponent<ActionScript>();
        stats = animator.GetComponent<PlayerStats>();
        item = animator.GetComponentInChildren<ItemManager>();

        float animDuration = stats.attackCooldown;

        attackDelay = animDuration * (1 - hitTiming);

        action.attackDisableTime = Time.time + attackDelay;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ( action.isAttack) return;

        float progress = stateInfo.normalizedTime % 1f;
        currentLoop = Mathf.FloorToInt(stateInfo.normalizedTime);

        if (currentLoop > lastLoop)
        {
            lastLoop = currentLoop;
            hashitThisLoop = false;

        }

        if (!hashitThisLoop && progress >= hitTiming)
        {

            if (action.target != null && action.target.gameObject.activeInHierarchy)
            {
                stats.HealthTrigger();
                stats.ManaTrigger();
                action.targetParent.TakeDamageAll(0, stats.damage, 0, ArmorType.패기, true, stats.neutralizeDefense);
                foreach (List<Item> items in item.list.itemList)
                {
                    foreach (Item item in items)
                    {
                    }
                }


                Debug.Log("💥 타격 발생");
            }
            
            hashitThisLoop = true;
            lastLoop = currentLoop;
            action.attackDisableTime = Time.time + attackDelay;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hashitThisLoop)
            action.attackDisableTime = Time.time + stats.attackCooldown * (1 - stateInfo.normalizedTime % 1f);
        else
            action.attackDisableTime = 0;
    }
}
