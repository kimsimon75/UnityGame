using UnityEngine;
using UnityEngine.AI;

public class PlayerAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private ActionScript action;
    private PlayerStats stats;
    private Animator anim;
    private NavMeshAgent agent;
    void Start()
    {
        action = GetComponent<ActionScript>();
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (action.target[GameManager.Instance.originStatFor6.targetNumber] == null)
        {
            action.TriggerHold();
            return;
        }
        Transform target = action.target[GameManager.Instance.originStatFor6.targetNumber];
        float dist = Vector3.Distance(transform.position, target.position);
        
        
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, UnityEngine.Time.deltaTime * 10f);


        if (dist > stats.detectRange && !action.IsAttackDisabledFor(GameManager.Instance.originStatFor6.targetNumber))
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") &&
            !(anim.GetNextAnimatorStateInfo(0).IsName("Walking") && anim.IsInTransition(0)))
            
            anim.CrossFade("Walking", GameManager.Instance.originStatFor6.blendingTime);
        }
        else
        {
            if (!action.IsAttackDisabledFor(GameManager.Instance.originStatFor6.targetNumber))
            {
                agent.isStopped = true;
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                !(anim.IsInTransition(0) &&
                anim.GetNextAnimatorStateInfo(0).IsName("Attack")))
                anim.CrossFade("Attack", GameManager.Instance.originStatFor6.blendingTime);
            }
        }
    }
}
