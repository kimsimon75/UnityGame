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
    private float hitTiming = .4f;
    private int currentLoop;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastLoop = -1;
        hashitThisLoop = false;
        if(action == null)action = animator.GetComponent<ActionScript>();
        if(stats == null)stats = animator.GetComponent<PlayerStats>();
        if(game == null)game = stats.GetComponentInParent<GameManager>();
        item = game.ItemManager;

        action.attackDisableTime = Time.time + attackDelay;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ( action.isAttack) return;

        float animDuration = stats.attackCooldown;

        attackDelay = animDuration * (1 - hitTiming);

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

                if (item == null)
                    Debug.Log("None item");
                foreach (Item items in item.list.currentItem)
                {
                    items.skill.Invoke(action.target.GetComponent<Actor>(), items);
                }
                item.Clear(item.editItem, false);

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
