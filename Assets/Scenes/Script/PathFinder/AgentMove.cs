using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class AgentMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private ActionScript action;

    public RaycastHit LastRaycastHit { get; private set; }  // 외부 접근용 프로퍼티

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.autoBraking = false;  // autoBraking을 true로 설정

        action = GetComponent<ActionScript>();
        agent.speed = GetComponent<PlayerStats>().MoveSpeed;
    }

    void Update()
    {
        NavMeshHit point = action.point;
        float dist = Vector3.Distance(transform.position, point.position);
        if (dist > 0.1f)
        {
            Vector3 direction = (point.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, UnityEngine.Time.deltaTime * 10f);
            agent.SetDestination(point.position);
        }
        else
        {
            Debug.Log(action.isAttack);
            agent.ResetPath();
            if (action.isAttack)
                action.TriggerStop();
            else
                action.TriggerHold();
        }
    }

    void FixedUpdate()
    {
    }
}