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
        if (action.target == null)
        {
            action.TriggerHold();
            return;
        }
        Transform target = action.target;
        float dist = Vector3.Distance(transform.position, target.position);
        
        
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, UnityEngine.Time.deltaTime * 10f);


        if (dist > stats.detectRange - 0.1f && !action.isAttack)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Walking") &&
            !(anim.GetNextAnimatorStateInfo(0).IsName("Walking") &&
             anim.IsInTransition(0)))
            
            anim.CrossFade("Walking", stats.blendingTime);
        }
        else
        {
            if (!action.isAttack)
            {
                agent.isStopped = true;
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack") &&
                !(anim.IsInTransition(0) &&
                anim.GetNextAnimatorStateInfo(0).IsName("Base Layer.Attack")))
                anim.CrossFade("Attack", stats.blendingTime);
            }
        }
    }
}
