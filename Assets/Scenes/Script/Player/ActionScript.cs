using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UIElements;

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
    public Actor targetParent = null;
    private bool OnTheStory = false;
    public Transform statsTarget = null;
    [SerializeField] private Vector3 offset = new Vector3(0f, 11f, -11f);
    public Camera mainCamera;

    public Transform StoryCannon;
    public Transform MagicZone;

    private float zoomSpeed = 10f;
    private float minDistance = 30f;
    private float maxDistance = 80f;
    private Vector3 camOffset = new Vector3(0, 12f, -6f);
    private float targetDistance;
    private float zoomVelocity;
    private float smoothTimeZoom = 0.10f;

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
        targetDistance = mainCamera.fieldOfView;
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 12f, transform.position.z - 6f);

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
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (isReady)
            {
                LayerMask enemyLayer = LayerMask.GetMask("Enemy");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, enemyLayer))
                {
                    SetFunction(hitInfo.transform);
                    TriggerAttack();
                }
                else
                {
                    TriggerHold();
                    isReady = false;
                }
            }
            else
            {
                LayerMask mask = LayerMask.GetMask("Enemy", "Cannon");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, mask))
                {
                    statsTarget = hitInfo.transform;
                }

            }

        }
        else if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, enemyLayer))
            {
                SetFunction(hitInfo.transform);
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
        else if (Input.GetKeyDown(KeyCode.G))
        {
            OnTheStory = !OnTheStory;
            Transform Goal;

            if (OnTheStory)
            {
                Goal = StoryCannon.transform;
                mainCamera.transform.position = new Vector3(Goal.position.x, Goal.position.y - 1f + 0.083333349f, Goal.position.z) + camOffset;
            }
            else
            {
                Goal = MagicZone.transform;
                mainCamera.transform.position = new Vector3(Goal.position.x, Goal.position.y + 0.073333349f, Goal.position.z) + camOffset;
            }
            TriggerHold();
            point = default;
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.Warp(Goal.position);   // NavMeshAgent 내부 좌표까지 동기화
                agent.ResetPath();           // 남아 있던 경로 제거 (선택)
            }
            else
            {
                // Agent 가 꺼져 있거나 아직 NavMesh 위가 아니면 transform 직접 이동
                transform.position = Goal.position;
            }
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z) + camOffset;
        }

        float scroll = -Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            targetDistance = Mathf.Clamp(
                targetDistance + scroll * zoomSpeed,
                minDistance,
                maxDistance);
        }
    }

    void LateUpdate()
    {
        mainCamera.fieldOfView = Mathf.SmoothDamp(
            mainCamera.fieldOfView,
            targetDistance,
            ref zoomVelocity,
            smoothTimeZoom
        );

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

    public void SetFunction(Transform hitInfo)
    {
        target = hitInfo;
        if (target.GetComponent<EnemyStats>() != null)
            targetParent = target.GetComponent<EnemyStats>();
        else if (target.GetComponent<Story>() != null)
            targetParent = target.GetComponent<Story>();
    }
}
