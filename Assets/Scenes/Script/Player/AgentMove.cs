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
    Vector3 _lastPos;
    float _stuckTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.autoBraking = false;  // autoBraking을 true로 설정

        action = GetComponent<ActionScript>();
        agent.speed = GetComponent<PlayerStats>().MoveSpeed;
        agent.updateRotation = false;
        _lastPos = transform.position;
    }

    void Update()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        Vector3 goal = action.point.position;

        if (!NavMesh.SamplePosition(goal, out var goalHit, 1.0f, agent.areaMask))
        {
            StopAndHold();
            return;
        }
        goal = goalHit.position;

        var path = new NavMeshPath();
        bool found = agent.CalculatePath(goal, path);

        if (!found || path.status == NavMeshPathStatus.PathInvalid)
        {
            StopAndHold();
            return;
        }

        Vector3 dest = goal;
        if (path.status == NavMeshPathStatus.PathPartial && path.corners.Length > 0)
            dest = path.corners[path.corners.Length - 1];

        // ✅ Hold에서 멈춘 걸 다시 풀어줘야 함

        if ((agent.destination - dest).sqrMagnitude > 0.01f)
            agent.SetDestination(dest);

        // ✅ 회전: 남이 준 goal 말고 "agent가 실제로 가려는 방향"으로 돌리는 게 안정적
        Vector3 dir = agent.desiredVelocity;
        if (dir.sqrMagnitude > 0.001f)
        {
            dir.y = 0f;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            StopAndHold();
            return;
        }
    }

    void StopAndHold()
    {
        agent.ResetPath();              // 경로 제거 :contentReference[oaicite:2]{index=2}
        agent.isStopped = true;         // 멈춤
        action.TriggerHold();           // 너가 쓰는 홀드 진입
    }
}