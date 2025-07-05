using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class ActionScript : MonoBehaviour
{
    Animator anim;
    private bool isReady = false;
    private PlayerAttack attack;
    private HoldScanner hold;
    private NavMeshAgent agent;
    private AgentMove move;
    private PlayerStats stats;
    public Transform target = null;
    public NavMeshHit point;
    public float attackDisableTime = 0f;
    public bool isAttack => Time.time < attackDisableTime;    // 기존 플래그
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        attack = GetComponent<PlayerAttack>();
        hold = GetComponent<HoldScanner>();
        agent = GetComponent<NavMeshAgent>();
        move = GetComponent<AgentMove>();
        stats = GetComponent<PlayerStats>();

        TriggerHold();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.S))
        {
            TriggerStop();
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            TriggerHold();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            isReady = true;
        }
        else if (Input.GetMouseButtonDown(0) && isReady && !EventSystem.current.IsPointerOverGameObject())
        {
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, enemyLayer))
            {
                target = hitInfo.transform;
                TriggerAttack();
            }
            else
                isReady = false;
        }
        else if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, enemyLayer))
            {
                target = hitInfo.transform;
                TriggerAttack();
            }
            else if (Physics.Raycast(ray, out RaycastHit groundHit, 100f))
            {
                if (NavMesh.SamplePosition(groundHit.point, out NavMeshHit navHit, 1.0f, agent.areaMask))
                {
                    point = navHit;
                    TriggerMove();
                }
            }
        }
    }

    public void TriggerAttack()
    {
        attack.enabled = true;
        hold.enabled = false;
        agent.ResetPath();

        agent.isStopped = false;
        move.enabled = false;

        isReady = false;
    }

    public void TriggerHold()
    {
        attack.enabled = false;
        hold.enabled = true;
        agent.isStopped = true;
        move.enabled = false;

        anim.CrossFade("Idle", stats.blendingTime);
    }

    public void TriggerMove()
    {
        attack.enabled = false;
        hold.enabled = false;
        agent.isStopped = false;
        move.enabled = true;

        target = null;
        anim.CrossFade("Walking", stats.blendingTime);

    }

    public void TriggerStop()
    {
        attack.enabled = false;
        hold.enabled = false;
        agent.isStopped = true;
        target = null;
        anim.CrossFade("Idle", stats.blendingTime);
    }
}
